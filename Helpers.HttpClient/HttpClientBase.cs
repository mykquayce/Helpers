﻿using Dawn;
using Helpers.Common;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.HttpClient
{
	public abstract class HttpClientBase
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			IgnoreNullValues = false,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		private readonly string _name;
		private readonly HttpMessageInvoker _httpMessageInvoker;
		private readonly ILogger? _logger;
		private readonly ITracer? _tracer;

		protected HttpClientBase(
			IHttpClientFactory httpClientFactory,
			ILogger? logger = default,
			ITracer? tracer = default)
		{
			Guard.Argument(() => httpClientFactory).NotNull();

			_logger = logger;
			_tracer = tracer;

			_name = this.GetType().Name;

			var httpClient = httpClientFactory.CreateClient(_name);

			Guard.Argument(() => httpClient).NotNull();
			Guard.Argument(() => httpClient.BaseAddress)
				.NotNull()
				.Require(u => !string.IsNullOrWhiteSpace(u.OriginalString), _ => nameof(httpClientFactory) + " has a blank base address");

			_httpMessageInvoker = httpClient;
		}

		public void Dispose()
		{
			_httpMessageInvoker?.Dispose();
		}

		protected async Task<(HttpStatusCode, T, IDictionary<string, IEnumerable<string>>)> SendAsync<T>(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? methodName = default)
		{
			var (httpStatusCode, stream, headers) = await SendAsync(httpMethod, uri, body, methodName);

			var value = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

			return (httpStatusCode, value, headers);
		}

		protected async Task<(HttpStatusCode, Stream, IDictionary<string, IEnumerable<string>>)> SendAsync(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? methodName = default)
		{
			Guard.Argument(() => httpMethod).NotNull()
				.Require(m => !string.IsNullOrWhiteSpace(m.Method), _ => nameof(httpMethod) + " is blank");
			Guard.Argument(() => uri)
				.NotNull()
				.Require(u => !string.IsNullOrWhiteSpace(u.OriginalString), _ => nameof(uri) + " is blank");

			using var scope = _tracer?.BuildSpan($"{_name}=>{methodName}")
				.WithTag(nameof(httpMethod), httpMethod.Method)
				.WithTag(nameof(uri), uri.OriginalString)
				.WithTag(nameof(body), body)
				.StartActive(finishSpanOnDispose: true);

			_logger?.LogInformation(
				new Dictionary<string, object?>(3)
				{
					[nameof(httpMethod)] = httpMethod.Method,
					[nameof(uri)] = uri.OriginalString,
					[nameof(body)] = body?.Truncate(),
				}.ToKeyValuePairString());

			var httpRequestMessage = new HttpRequestMessage(httpMethod, uri);

			if (!string.IsNullOrWhiteSpace(body))
			{
				var requestContent = new StringContent(body, Encoding.UTF8, "application/json");

				httpRequestMessage.Content = requestContent;
			}

			HttpResponseMessage httpResponseMessage;

			try
			{
				httpResponseMessage = await _httpMessageInvoker.SendAsync(httpRequestMessage, CancellationToken.None);
			}
			catch (Exception exception)
			{
				exception.Data.Add(nameof(httpMethod), httpMethod.Method);
				exception.Data.Add(nameof(uri), uri.OriginalString);
				exception.Data.Add(nameof(body), body);

				string? errorObject = default;

				if (exception is HttpRequestException
					&& string.Equals(exception.Message, "The requested name is valid, but no data of the requested type was found.", StringComparison.InvariantCultureIgnoreCase))
				{
					errorObject = exception.ToString();
				}
				else
				{
					errorObject = JsonSerializer.Serialize(exception, _jsonSerializerOptions);
				}

				scope?.Span
					.SetTag(OpenTracing.Tag.Tags.Error, true)
					.Log(
						new Dictionary<string, object>(5)
						{
							[LogFields.ErrorKind] = exception.GetType().FullName,
							[LogFields.ErrorObject] = errorObject,
							[LogFields.Event] = OpenTracing.Tag.Tags.Error.Key,
							[LogFields.Message] = exception.Message,
							[LogFields.Stack] = exception.StackTrace,
						});

				_logger?.LogError(exception, "{0}={1}, {2}={3}, {4}={5}", nameof(httpMethod), httpMethod.Method, nameof(uri), uri.OriginalString, nameof(body), body);

				throw;
			}

			var responseStatusCode = httpResponseMessage.StatusCode;
			var responseContent = await httpResponseMessage.Content.ReadAsStreamAsync();

			var headers = new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase)
				.AddRange(httpResponseMessage.Headers)
				.AddRange(httpResponseMessage.Content.Headers);

			return (responseStatusCode, responseContent, headers);
		}
	}

	public readonly struct SimpleException
	{
		public SimpleException(Exception exception)
		{
			Data = new Dictionary<object, object>();
			InnerExceptions = new List<SimpleException>();
			Message = exception.Message;
			StackTrace = exception.StackTrace;
			TypeName = exception.GetType().FullName;

			if (exception.InnerException != default)
			{
				InnerException = new SimpleException(exception.InnerException);
			}
			else
			{
				InnerException = default;
			}

			foreach (var key in exception.Data.Keys)
			{
				Data.Add(key, exception.Data[key]);
			}

			switch (exception)
			{
				case AggregateException aggregateException:
					foreach (var inner in aggregateException.InnerExceptions)
					{
						InnerExceptions.Add(new SimpleException(inner));
					}
					break;
			}
		}

		public string Message { get; }
		public string StackTrace { get; }
		public string TypeName { get; }
		public IDictionary<object, object> Data { get; }
		public SimpleException? InnerException { get; }
		public ICollection<SimpleException> InnerExceptions { get; }
	}
}
