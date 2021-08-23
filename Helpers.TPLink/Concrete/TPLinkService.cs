﻿using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.TPLink.Concrete
{
	public class TPLinkService : ITPLinkService
	{
		private readonly ITPLinkClient _client;
		private readonly IDeviceCache _cache = new DeviceCache();
		private delegate bool TryGetDelegate<T>(T value, [MaybeNullWhen(false)] out Models.Device device);

		public TPLinkService() : this(Config.Defaults) { }
		public TPLinkService(IOptions<Config> options) : this(options.Value) { }
		public TPLinkService(Config config) : this(new TPLinkClient(config)) { }

		public TPLinkService(ITPLinkClient client)
		{
			_client = client;
		}

		public Task<Models.RealtimeData> GetRealtimeDataAsync(string alias) => GetRealtimeDataAsync(_cache.TryGetValue, alias);
		public Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip) => GetRealtimeDataAsync(_cache.TryGetValue, ip);
		public Task<Models.RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac) => GetRealtimeDataAsync(_cache.TryGetValue, mac);

		private async Task<Models.RealtimeData> GetRealtimeDataAsync<T>(TryGetDelegate<T> tryGet, T value)
		{
			if (!tryGet(value, out var device))
			{
				await foreach (var item in _client.DiscoverAsync())
				{
					_cache.Add(item);
				}

				if (!tryGet(value, out device))
				{
					throw new KeyNotFoundException(value + " not found");
				}
			}

			return await _client.GetRealtimeDataAsync(device.IPAddress);
		}
	}
}