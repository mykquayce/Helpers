using Dawn;
using Helpers.Common;
using Helpers.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Web
{
	public abstract class WebClientBase
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
		{
			AllowTrailingCommas = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		private readonly HttpMessageInvoker _httpMessageInvoker;
		private readonly ILogger? _logger;
		private readonly ITracer? _tracer;

		protected WebClientBase(
			HttpClient httpClient,
			ILogger? logger = default,
			ITracer? tracer = default)
			: this(logger, tracer)
		{
			_httpMessageInvoker = Guard.Argument(httpClient).NotNull().Value;
		}

		protected WebClientBase(
			IHttpClientFactory httpClientFactory,
			ILogger? logger = default,
			ITracer? tracer = default)
			: this(logger, tracer)
		{
			_httpMessageInvoker = httpClientFactory.CreateClient(this.GetType().Name);
		}

		protected WebClientBase(
			ILogger? logger = default,
			ITracer? tracer = default)
		{
			_logger = logger;
			_tracer = tracer;
			_httpMessageInvoker = new HttpClient();
		}

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

			T o;

			if (typeof(T) == typeof(string))
			{
				using var reader = new StreamReader(stream);
				var s = await reader.ReadToEndAsync();
				o = (T)Convert.ChangeType(s, TypeCode.String);
			}
			else
			{
				o = await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions)
					?? throw new Exception();
			}

			return new Models.Concrete.Response<T>(response.Headers, response.StatusCode, o);
		}

		protected async Task<Models.IResponse> SendAsync(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? callerMemberName = default,
			[CallerFilePath] string? callerFilePath = default)
		{
			Guard.Argument(httpMethod).NotNull()
				.Wrap(m => m.Method).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(uri).NotNull()
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
			Guard.Argument(request).NotNull()
				.Wrap(r => r.RequestUri).NotEqual(default)
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

			return new Models.Concrete.Response(headers, response.StatusCode, response.Content.ReadAsStreamAsync());
		}
	}
}
