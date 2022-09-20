using Dawn;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Concrete;

public class TPLinkService : ITPLinkService
{
	private readonly ITPLinkClient _client;
	private readonly IDeviceCache _cache;
	private delegate bool TryGetDelegate<T>(T value, [MaybeNullWhen(false)] out Models.Device device);

	public TPLinkService(ITPLinkClient client, IDeviceCache cache)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_cache = Guard.Argument(cache).NotNull().Value;
	}

	public async IAsyncEnumerable<Models.Device> DicoveryAsync()
	{
		await foreach (var item in _client.DiscoverAsync())
		{
			_cache.Add(item);
			yield return item;
		}
	}

	#region getrealtimedata
	public async Task<Models.RealtimeData> GetRealtimeDataAsync(string alias)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, alias);
		return await _client.GetRealtimeDataAsync(device.IPAddress);
	}

	public Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip)
		=> _client.GetRealtimeDataAsync(ip);

	public async Task<Models.RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, mac);
		return await _client.GetRealtimeDataAsync(device.IPAddress);
	}
	#endregion getrealtimedata

	#region getsysteminfo
	public Task<Models.SystemInfo> GetSystemInfoAsync(IPAddress ip)
		=> _client.GetSystemInfoAsync(ip);

	public async Task<Models.SystemInfo> GetSystemInfoAsync(PhysicalAddress mac)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, mac);
		return await _client.GetSystemInfoAsync(device.IPAddress);
	}

	public async Task<Models.SystemInfo> GetSystemInfoAsync(string alias)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, alias);
		return await _client.GetSystemInfoAsync(device.IPAddress);
	}
	#endregion getsysteminfo

	#region getstate
	public Task<bool> GetStateAsync(IPAddress ip)
		=> _client.GetStateAsync(ip);

	public async Task<bool> GetStateAsync(PhysicalAddress mac)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, mac);
		return await _client.GetStateAsync(device.IPAddress);
	}

	public async Task<bool> GetStateAsync(string alias)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, alias);
		return await _client.GetStateAsync(device.IPAddress);
	}
	#endregion getstate

	#region setstate
	public Task SetStateAsync(IPAddress ip, bool state)
		=> _client.SetStateAsync(ip, state);

	public async Task SetStateAsync(PhysicalAddress mac, bool state)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, mac);
		await _client.SetStateAsync(device.IPAddress, state);
	}

	public async Task SetStateAsync(string alias, bool state)
	{
		var device = await GetDeviceAsync(_cache.TryGetValue, alias);
		await _client.SetStateAsync(device.IPAddress, state);
	}
	#endregion setstate

	private async Task<Models.Device> GetDeviceAsync<T>(TryGetDelegate<T> tryGet, T value)
	{
		if (tryGet(value, out var device))
		{
			return device;
		}

		await foreach (var item in _client.DiscoverAsync())
		{
			_cache.Add(item);
		}

		if (tryGet(value, out device))
		{
			return device;
		}

		throw new KeyNotFoundException(value + " not found");
	}
}
