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

	private async Task<int> ResolveGroupNameAsync(string name, CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue("group_" + name.ToLowerInvariant(), out int? index))
		{
			return index!.Value;
		}

		await RefreshCacheAsync(cancellationToken);

		if (_memoryCache.TryGetValue("group_" + name.ToLowerInvariant(), out index))
		{
			return index!.Value;
		}

		throw new KeyNotFoundException(name + " not found");
	}

	private async Task<int> ResolveLightAliasAsync(string alias, CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue("light_" + alias.ToLowerInvariant(), out int? index))
		{
			return index!.Value;
		}

		await RefreshCacheAsync(cancellationToken);

		if (_memoryCache.TryGetValue("light_" + alias.ToLowerInvariant(), out index))
		{
			return index!.Value;
		}

		throw new KeyNotFoundException(alias + " not found");
	}

	private async Task<string> ResolveSceneNameAsync(string name, CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue("scene_" + name.ToLowerInvariant(), out string? id))
		{
			return id!;
		}

		await RefreshCacheAsync(cancellationToken);

		if (_memoryCache.TryGetValue("scene_" + name.ToLowerInvariant(), out id))
		{
			return id!;
		}

		throw new KeyNotFoundException(name + " not found");
	}
}
