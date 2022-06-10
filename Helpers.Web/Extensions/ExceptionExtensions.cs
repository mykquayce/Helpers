namespace System;

public static class ExceptionExtensions
{
	public async static Task<Exception> PopulateExceptionAsync(this Exception exception, HttpRequestMessage request)
	{
		var method = request.Method;
		var requestUri = request.RequestUri.OriginalString;

		exception.Data
			.TryAdd(nameof(method), method)
			.TryAdd(nameof(requestUri), requestUri);

		if (request.Content != null)
		{
			await using var stream = await request.Content.ReadAsStreamAsync();
			exception.PopulateException(stream);
		}

		return exception;
	}

	public static Exception PopulateException(this Exception exception, Stream stream)
	{
		var body = stream.ReadAsString();

		if (string.IsNullOrWhiteSpace(body))
		{
			return exception;
		}

		exception.Data.TryAdd(nameof(body), body);

		return exception;
	}
}
