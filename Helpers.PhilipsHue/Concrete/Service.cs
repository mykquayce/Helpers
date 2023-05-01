using Microsoft.Extensions.Caching.Memory;

namespace Helpers.PhilipsHue.Concrete;

public partial class Service : IService
{
	private readonly IClient _client;
	private readonly IMemoryCache _memoryCache;

	public Service(IClient client, IMemoryCache memoryCache)
	{
		_client = client;
		_memoryCache = memoryCache;
	}

	private async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
	{
		var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
		await foreach (var (alias, index) in _client.GetLightAliasesAsync(cancellationToken))
		{
			_memoryCache.Set("light_" + alias, index, absoluteExpiration);
		}
	}

	private async Task<int> ResolveLightAliasAsync(string alias, CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue("light_" + alias, out int? index))
		{
			return index!.Value;
		}

		await RefreshCacheAsync(cancellationToken);

		if (_memoryCache.TryGetValue("light_" + alias, out index))
		{
			return index!.Value;
		}

		throw new KeyNotFoundException(alias + " not found");
	}
}
