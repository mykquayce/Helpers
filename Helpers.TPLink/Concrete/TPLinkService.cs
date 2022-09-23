using Dawn;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Concrete;

public class TPLinkService : ITPLinkService
{
	private readonly ITPLinkClient _client;
	private readonly IMemoryCache _cache;

	public TPLinkService(ITPLinkClient client, IMemoryCache cache)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_cache = Guard.Argument(cache).NotNull().Value;
	}

	public async IAsyncEnumerable<Models.Device> DicoveryAsync()
	{
		var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
		await foreach (var item in _client.DiscoverAsync())
		{
			_cache.Set(item.Alias, item, absoluteExpiration);
			_cache.Set(item.IPAddress, item, absoluteExpiration);
			_cache.Set(item.PhysicalAddress, item, absoluteExpiration);
			yield return item;
		}
	}

	#region getrealtimedata
	public async Task<Models.RealtimeData> GetRealtimeDataAsync(string alias)
	{
		var device = await GetDeviceAsync(alias);
		return await _client.GetRealtimeDataAsync(device.IPAddress);
	}

	public Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip)
		=> _client.GetRealtimeDataAsync(ip);

	public async Task<Models.RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac)
	{
		var device = await GetDeviceAsync(mac);
		return await _client.GetRealtimeDataAsync(device.IPAddress);
	}
	#endregion getrealtimedata

	#region getsysteminfo
	public Task<Models.SystemInfo> GetSystemInfoAsync(IPAddress ip)
		=> _client.GetSystemInfoAsync(ip);

	public async Task<Models.SystemInfo> GetSystemInfoAsync(PhysicalAddress mac)
	{
		var device = await GetDeviceAsync(mac);
		return await _client.GetSystemInfoAsync(device.IPAddress);
	}

	public async Task<Models.SystemInfo> GetSystemInfoAsync(string alias)
	{
		var device = await GetDeviceAsync(alias);
		return await _client.GetSystemInfoAsync(device.IPAddress);
	}
	#endregion getsysteminfo

	#region getstate
	public Task<bool> GetStateAsync(IPAddress ip)
		=> _client.GetStateAsync(ip);

	public async Task<bool> GetStateAsync(PhysicalAddress mac)
	{
		var device = await GetDeviceAsync(mac);
		return await _client.GetStateAsync(device.IPAddress);
	}

	public async Task<bool> GetStateAsync(string alias)
	{
		var device = await GetDeviceAsync(alias);
		return await _client.GetStateAsync(device.IPAddress);
	}
	#endregion getstate

	#region setstate
	public Task SetStateAsync(IPAddress ip, bool state)
		=> _client.SetStateAsync(ip, state);

	public async Task SetStateAsync(PhysicalAddress mac, bool state)
	{
		var device = await GetDeviceAsync(mac);
		await _client.SetStateAsync(device.IPAddress, state);
	}

	public async Task SetStateAsync(string alias, bool state)
	{
		var device = await GetDeviceAsync(alias);
		await _client.SetStateAsync(device.IPAddress, state);
	}
	#endregion setstate

	private async Task<Models.Device> GetDeviceAsync(object key)
	{
		if (_cache.TryGetValue(key, out Models.Device? device))
		{
			return device!;
		}

		await DicoveryAsync().ToListAsync();

		if (_cache.TryGetValue(key, out device))
		{
			return device!;
		}

		throw new KeyNotFoundException(key + " not found");
	}
}
