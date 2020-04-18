using Dawn;
using Helpers.Common;
using Helpers.Tracing;
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
	public abstract class HttpClientBase : IDisposable
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
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
			Dispose(disposing: true);
			GC.SuppressFinalize(obj: this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_httpMessageInvoker?.Dispose();
			}
		}

		protected async Task<(HttpStatusCode, T, IDictionary<string, IEnumerable<string>>)> SendAsync<T>(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? callerMemberName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			var response = await SendAsync(httpMethod, uri, body, callerMemberName, callerFilePath);

			var value = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

			return (httpStatusCode, value, headers);
		}

		protected async Task<(HttpStatusCode, Stream, IDictionary<string, IEnumerable<string>>)> SendAsync(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? callerMemberName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			Guard.Argument(() => httpMethod).NotNull()
				.Wrap(m => m.Method).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => uri).NotNull()
				.Wrap(u => u.OriginalString).NotNull().NotEmpty().NotWhiteSpace();

			using var scope = _tracer?.StartSpan(callerMemberName, callerFilePath);
			scope?.Span.SetTag(nameof(httpMethod), httpMethod.Method);
			scope?.Span.SetTag(nameof(uri), uri.OriginalString);
			scope?.Span.SetTag(nameof(body), body);

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
				var baseAddress = (_httpMessageInvoker as System.Net.Http.HttpClient)?.BaseAddress;

				exception.Data.Add(nameof(httpMethod), httpMethod.Method);
				exception.Data.Add(nameof(baseAddress), baseAddress);
				exception.Data.Add(nameof(uri), uri.OriginalString);
				exception.Data.Add(nameof(body), body);

				scope?.Span.Log(exception);

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
}
