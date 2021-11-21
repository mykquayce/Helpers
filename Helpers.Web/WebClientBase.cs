﻿using Dawn;
using Helpers.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OpenTracing;
using System.Runtime.CompilerServices;
using System.Text;

namespace Helpers.Web;

public abstract class WebClientBase
{
	private readonly HttpMessageInvoker _httpMessageInvoker;
	private readonly ILogger? _logger;
	private readonly ITracer? _tracer;

	#region constructors
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
	#endregion constructors

	#region protected methods
	protected async Task<Models.IResponse<T>> SendAsync<T>(
		HttpMethod httpMethod,
		Uri uri,
		string? body = default,
		CancellationToken? cancellationToken = default,
		[CallerMemberName] string? callerMemberName = default,
		[CallerFilePath] string? callerFilePath = default)
		where T : class
	{
		using var scope = LogAndTrace(callerMemberName, callerFilePath, httpMethod, uri, body);
		var request = BuildRequest(httpMethod, uri, body);
		var response = await SendRequestAsync(request, cancellationToken);
		return await ProcessResponseMessageAsync<T>(response, cancellationToken);
	}

	protected async Task<Models.IResponse> SendAsync(
		HttpMethod httpMethod,
		Uri uri,
		string? body = default,
		CancellationToken? cancellationToken = default,
		[CallerMemberName] string? callerMemberName = default,
		[CallerFilePath] string? callerFilePath = default)
	{
		using var scope = LogAndTrace(callerMemberName, callerFilePath, httpMethod, uri, body);
		var request = BuildRequest(httpMethod, uri, body);
		var response = await SendRequestAsync(request, cancellationToken);
		return ProcessResponseMessage(response);
	}

	protected async Task<Models.IResponse> SendAsync(
		HttpRequestMessage requestMessage,
		CancellationToken? cancellationToken = default,
		[CallerMemberName] string? callerMemberName = default,
		[CallerFilePath] string? callerFilePath = default)
	{
		using var scope = LogAndTrace(callerMemberName, callerFilePath, requestMessage);
		var response = await SendRequestAsync(requestMessage, cancellationToken);
		return ProcessResponseMessage(response);
	}

	protected async Task<Models.IResponse<T>> SendAsync<T>(
		HttpRequestMessage requestMessage,
		CancellationToken? cancellationToken = default,
		[CallerMemberName] string? callerMemberName = default,
		[CallerFilePath] string? callerFilePath = default)
		where T : class
	{
		using var scope = LogAndTrace(callerMemberName, callerFilePath, requestMessage);
		var response = await SendRequestAsync(requestMessage, cancellationToken);
		return await ProcessResponseMessageAsync<T>(response, cancellationToken);
	}
	#endregion protected methods

	#region private methods
	private IScope? LogAndTrace(string? callerMemberName, string? callerFilePath, HttpMethod httpMethod, Uri uri, string? body)
	{
		var scope = LogAndTrace(callerMemberName, callerFilePath);
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

		return scope;
	}

	private IScope? LogAndTrace(string? callerMemberName, string? callerFilePath, HttpRequestMessage requestMessage)
	{
		return LogAndTrace(callerMemberName, callerFilePath, requestMessage.Method, requestMessage.RequestUri, default);
	}

	private IScope? LogAndTrace(string? callerMemberName, string? callerFilePath)
	{
		return _tracer?.StartSpan(callerMemberName, callerFilePath);
	}

	private HttpRequestMessage BuildRequest(HttpMethod method, Uri uri, string? body = default)
	{
		var httpRequestMessage = new HttpRequestMessage(method, uri);

		if (!string.IsNullOrWhiteSpace(body))
		{
			var requestContent = new StringContent(body, Encoding.UTF8, "application/json");

			httpRequestMessage.Content = requestContent;
		}

		return httpRequestMessage;
	}

	private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken? cancellationToken = default)
	{
		try
		{
			return await _httpMessageInvoker.SendAsync(request, cancellationToken ?? CancellationToken.None);
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
	}

	private Models.IResponse ProcessResponseMessage(HttpResponseMessage response)
	{
		var headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);

		foreach (var (key, values) in response.Headers)
		{
			headers.Add(key, values.ToArray());
		}

		foreach (var (key, values) in response.Content.Headers)
		{
			headers.Add(key, values.ToArray());
		}

		return new Models.Concrete.Response(headers, response.StatusCode, response.Content.ReadAsStreamAsync());
	}

	private async Task<Models.IResponse<T>> ProcessResponseMessageAsync<T>(HttpResponseMessage response, CancellationToken? cancellationToken = default)
		where T : class
	{
		var (headers, statusCode, taskStream) = ProcessResponseMessage(response);

		await using var stream = await taskStream!;

		T o;

		if (typeof(T) == typeof(string))
		{
			using var reader = new StreamReader(stream);
			var s = await reader.ReadToEndAsync();
			o = (T)Convert.ChangeType(s, TypeCode.String);
		}
		else
		{
			o = await stream.DeserializeAsync<T>(cancellationToken)
				?? throw new Exception();
		}

		return new Models.Concrete.Response<T>(headers, statusCode, o);
	}
	#endregion private methods
}
