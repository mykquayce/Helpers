using Microsoft.Extensions.Caching.Memory;

namespace System.Net.Http;

public class CachingHandler(IMemoryCache memoryCache) : DelegatingHandler
{
	protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var key = request.RequestUri?.OriginalString.ToLowerInvariant();

		if (string.IsNullOrEmpty(key))
		{
			return await base.SendAsync(request, cancellationToken);
		}

		if (memoryCache.TryGetValue<(HttpStatusCode, byte[])>(key, out var tuple))
		{
			var (code, body) = tuple;
			return new(code) { Content = new ByteArrayContent(body), };
		}

		var response = await base.SendAsync(request, cancellationToken);

		if (response.IsSuccessStatusCode)
		{
			var code = response.StatusCode;
			var body = await response.Content.ReadAsByteArrayAsync(cancellationToken);
			var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(.9);
			memoryCache.Set(key, (code, body), absoluteExpiration);
		}

		return response;
	}
}
