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
			_memoryCache.Set("light_" + alias.ToLowerInvariant(), index, absoluteExpiration);
		}
		await foreach (var (name, id) in _client.GetGroupsAsync(cancellationToken))
		{
			_memoryCache.Set("group_" + name.ToLowerInvariant(), id, absoluteExpiration);
		}
		await foreach (var (name, id) in _client.GetScenesAsync(cancellationToken))
		{
			_memoryCache.Set("scene_" + name.ToLowerInvariant(), id, absoluteExpiration);
		}
	}

	private Task<int> ResolveGroupNameAsync(string name, CancellationToken cancellationToken = default)
		=> ResolveKeyAsync<int>("group_" + name, cancellationToken);

	private Task<int> ResolveLightAliasAsync(string alias, CancellationToken cancellationToken = default)
		=> ResolveKeyAsync<int>("light_" + alias, cancellationToken);

	private Task<string> ResolveSceneNameAsync(string name, CancellationToken cancellationToken = default)
		=> ResolveKeyAsync<string>("scene_" + name, cancellationToken);

	private async Task<T> ResolveKeyAsync<T>(string key, CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue(key.ToLowerInvariant(), out T? value))
		{
			return value!;
		}

		await RefreshCacheAsync(cancellationToken);

		if (_memoryCache.TryGetValue(key.ToLowerInvariant(), out value))
		{
			return value!;
		}

		throw new KeyNotFoundException(key + " not found");
	}
}
