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

		var (code, body) = await memoryCache.GetOrCreateAsync(key, factory);

		return new(code) { Content = new ByteArrayContent(body), };

		async Task<(HttpStatusCode, byte[])> factory(ICacheEntry entry)
		{
			var response = await base.SendAsync(request, cancellationToken);
			entry.AbsoluteExpiration = response.IsSuccessStatusCode
				? DateTimeOffset.UtcNow.AddHours(.9)
				: DateTimeOffset.MinValue;
			var code = response.StatusCode;
			var body = await response.Content.ReadAsByteArrayAsync(cancellationToken);
			return (code, body);
		}
	}
}
