using Microsoft.Extensions.Caching.Memory;
using System.Drawing;

namespace Helpers.PhilipsHue.Concrete;

public class Service : IService
{
	private readonly IClient _client;
	private readonly IMemoryCache _memoryCache;

	public Service(IClient client, IMemoryCache memoryCache)
	{
		_client = client;
		_memoryCache = memoryCache;
	}

	public async IAsyncEnumerable<string> GetLightAliasesAsync(CancellationToken? cancellationToken = null)
	{
		await foreach (var (alias, _) in _client.GetLightAliasesAsync(cancellationToken))
		{
			yield return alias;
		}
	}

	#region brightness
	public async Task<float> GetLightBrightnessAsync(string alias, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		return await _client.GetLightBrightnessAsync(index, cancellationToken);
	}

	public async Task SetLightBrightnessAsync(string alias, float brightness, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		await _client.SetLightBrightnessAsync(index, brightness, cancellationToken);
	}
	#endregion brightness

	#region color
	public async Task<Color> GetLightColorAsync(string alias, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		return await _client.GetLightColorAsync(index, cancellationToken);
	}

	public async Task SetLightColorAsync(string alias, Color color, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		await _client.SetLightColorAsync(index, color, cancellationToken);
	}
	#endregion color

	#region power
	public async Task<bool> GetLightPowerAsync(string alias, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		return await _client.GetLightPowerAsync(index, cancellationToken);
	}

	public async Task SetLightPowerAsync(string alias, bool on, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		await _client.SetLightPowerAsync(index, on, cancellationToken);
	}
	#endregion power

	#region temperature
	public async Task<short> GetLightTemperatureAsync(string alias, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		return await _client.GetLightTemperatureAsync(index, cancellationToken);
	}

	public async Task SetLightTemperatureAsync(string alias, short kelvins, CancellationToken? cancellationToken = null)
	{
		var index = await ResolveAliasAsync(alias, cancellationToken);
		await _client.SetLightTemperatureAsync(index, kelvins, cancellationToken);
	}
	#endregion temperature

	private async Task RefreshCacheAsync(CancellationToken? cancellationToken = null)
	{
		var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
		await foreach (var (alias, index) in _client.GetLightAliasesAsync(cancellationToken))
		{
			_memoryCache.Set(alias, index, absoluteExpiration);
		}
	}

	private async Task<int> ResolveAliasAsync(string alias, CancellationToken? cancellationToken = null)
	{
		if (_memoryCache.TryGetValue(alias, out int? index))
		{
			return index!.Value;
		}

		await RefreshCacheAsync(cancellationToken);

		if (_memoryCache.TryGetValue(alias, out index))
		{
			return index!.Value;
		}

		throw new KeyNotFoundException(alias + " not found");
	}
}
