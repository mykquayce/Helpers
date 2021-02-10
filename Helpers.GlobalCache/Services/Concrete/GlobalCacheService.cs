using Dawn;
using Helpers.GlobalCache.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Services.Concrete
{
	public sealed class GlobalCacheService : IGlobalCacheService
	{
		public record Config
		{
			public ushort Port { get; init; } = 4_998;
		}

		private readonly static IDictionary<PhysicalAddress, IPAddress> _cache = new Dictionary<PhysicalAddress, IPAddress>();
		private readonly static Encoding _encoding = Encoding.UTF8;

		private readonly ushort _port;
		private readonly Clients.ISocketClient _socketClient;
		private readonly Clients.IUdpClient _udpClient;

		public GlobalCacheService(IOptions<Config> options, Clients.ISocketClient socketClient, Clients.IUdpClient udpClient)
			: this(options.Value, socketClient, udpClient)
		{ }

		public GlobalCacheService(Config config, Clients.ISocketClient socketClient, Clients.IUdpClient udpClient)
			: this(config.Port, socketClient, udpClient)
		{ }

		public GlobalCacheService(ushort port, Clients.ISocketClient socketClient, Clients.IUdpClient udpClient)
		{
			_port = Guard.Argument(() => port).NotEqual((ushort)0).Value;
			_socketClient = Guard.Argument(() => socketClient).NotNull().Value;
			_udpClient = Guard.Argument(() => udpClient).NotNull().Value;
		}

		public void Dispose() => _udpClient?.Dispose();

		public async IAsyncEnumerable<PhysicalAddress> DiscoverAsync()
		{
			var beacons = _udpClient.DiscoverAsync();

			await foreach (var beacon in beacons)
			{
				var mac = beacon.GetPhysicalAddress();
				var ip = beacon.GetIPAddress();
				_cache.TryAdd(mac, ip);
				yield return mac;
			}
		}

		public async Task SendMessageasync(PhysicalAddress mac, string message)
		{
			var ip = _cache[mac];
			var ipEndPoint = new IPEndPoint(ip, _port);

			var response = await ConnectSendReceiveAsync(ipEndPoint, message);

			if (response.StartsWith("completeir", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}

			throw new InvalidOperationException("Unexpected response: " + response)
			{
				Data =
				{
					[nameof(mac)] = mac.ToString(),
					[nameof(ip)] = ip.ToString(),
					[nameof(message)] = message,
				},
			};
		}

		private async Task<string> ConnectSendReceiveAsync(EndPoint endPoint, string message)
		{
			var messageBytes = _encoding.GetBytes(message);
			var responseBytes = await ConnectSendReceiveAsync(endPoint, messageBytes).ToArrayAsync();
			var response = _encoding.GetString(responseBytes);
			return response;
		}

		private async IAsyncEnumerable<byte> ConnectSendReceiveAsync(EndPoint endPoint, byte[] message)
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);

			await _socketClient.ConnectAsync(endPoint, cts.Token);

			var count = 3;

			while (--count >= 0)
			{
				await _socketClient.SendAsync(message, cts.Token);
				await Task.Delay(millisecondsDelay: 100);
			}

			await foreach (var @byte in _socketClient.ReceiveAsync(cts.Token))
			{
				yield return @byte;
			}
		}
	}
}
