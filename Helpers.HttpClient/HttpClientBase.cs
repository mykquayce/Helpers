using Dawn;
using Helpers.Common;
using Helpers.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
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

		private readonly HttpMessageInvoker _httpMessageInvoker;
		private readonly ILogger? _logger;
		private readonly ITracer? _tracer;

		protected HttpClientBase(
			IHttpClientFactory httpClientFactory,
			ILogger? logger = default,
			ITracer? tracer = default)
			: this(logger, tracer)
		{
			var name = this.GetType().Name;
			var httpClient = httpClientFactory.CreateClient(name);
			_httpMessageInvoker = Guard.Argument(() => httpClient).NotNull().Value;
		}

		protected HttpClientBase(
			System.Net.Http.HttpClient httpClient,
			ILogger? logger = default,
			ITracer? tracer = default)
			: this(logger, tracer)
		{
			_httpMessageInvoker = Guard.Argument(() => httpClient).NotNull().Value;
		}

		protected HttpClientBase(
			ILogger? logger = default,
			ITracer? tracer = default)
		{
			_logger = logger;
			_tracer = tracer;
			_httpMessageInvoker = new System.Net.Http.HttpClient();
		}

		#region IDisposable implementation
		private bool _disposedValue;
		public void Dispose()
		{
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_httpMessageInvoker?.Dispose();
				}

				_disposedValue = true;
			}
		}
		#endregion IDisposable implementation

		protected async Task<Models.IResponse<T>> SendAsync<T>(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? callerMemberName = default,
			[CallerFilePath] string? callerFilePath = default)
			where T : class
		{
			var response = await SendAsync(httpMethod, uri, body, callerMemberName, callerFilePath);

			using var stream = await response.TaskStream!;

			var o = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);

			return new Models.Concrete.Response<T>
			{
				Headers = response.Headers,
				StatusCode = response.StatusCode,
				Object = o,
			};
		}

		protected async Task<Models.IResponse> SendAsync(
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

			return await SendAsync(httpRequestMessage);
		}

		protected async Task<Models.IResponse> SendAsync(HttpRequestMessage request)
		{
			Guard.Argument(() => request).NotNull()
				.Wrap(r => r.RequestUri).NotNull()
				.Wrap(u => u.OriginalString).NotNull().NotEmpty().NotWhiteSpace();

			HttpResponseMessage response;

			try
			{
				response = await _httpMessageInvoker.SendAsync(request, CancellationToken.None);
			}
			catch (Exception exception)
			{
				var baseAddress = (_httpMessageInvoker as System.Net.Http.HttpClient)?.BaseAddress.OriginalString;

				string? body = request.Content is null
					? null
					: await request.Content.ReadAsStringAsync();

				exception.Data.Add(nameof(baseAddress), baseAddress);
				exception.Data.Add(nameof(body), body);
				exception.Data.Add(nameof(request.Method), request.Method);
				exception.Data.Add(nameof(request.RequestUri), request.RequestUri.OriginalString);

				_tracer?.ActiveSpan?.Log(exception);

				_logger?.LogError(exception,
					"{0}={1}, {2}={3}, {4}={5}",
					nameof(request.Method), request.Method,
					nameof(request.RequestUri), request.RequestUri.OriginalString,
					nameof(body), body);

				throw;
			}

			var headers = new Dictionary<string, StringValues>(StringComparer.InvariantCultureIgnoreCase);

			foreach (var (key, values) in response.Headers)
			{
				headers.Add(key, new StringValues(values.ToArray()));
			}

			foreach (var (key, values) in response.Content.Headers)
			{
				headers.Add(key, new StringValues(values.ToArray()));
			}

			return new Models.Concrete.Response
			{
				Headers = headers,
				StatusCode = response.StatusCode,
				TaskStream = response.Content.ReadAsStreamAsync(),
			};
		}
	}
}
