using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace System.Net.Http;

public class CachingHandler(IMemoryCache memoryCache, IOptions<CachingHandler.IConfig> config) : DelegatingHandler
{
	public interface IConfig { TimeSpan Expiration { get; set; } }

	private readonly TimeSpan _expiration = config.Value.Expiration;

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
			var absoluteExpiration = DateTimeOffset.UtcNow.Add(_expiration);
			memoryCache.Set(key, (code, body), absoluteExpiration);
		}

		return response;
	}
}
