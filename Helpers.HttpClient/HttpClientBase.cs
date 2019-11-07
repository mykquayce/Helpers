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
	public abstract class HttpClientBase
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
