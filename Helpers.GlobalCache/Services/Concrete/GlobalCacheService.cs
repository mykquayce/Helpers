using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Services.Concrete
{
	public class GlobalCacheService : IGlobalCacheService
	{
		public record Config(string BroadcastIPAddress, ushort Port)
		{
			public Config() : this("239.255.250.250", 4_998) { }
		}

		private readonly static IDictionary<string, IPAddress> _cache = new Dictionary<string, IPAddress>(StringComparer.InvariantCultureIgnoreCase);

		private readonly ushort _port;
		private readonly Helpers.Networking.Clients.ISocketClient _socketClient;

		#region constructors
		public GlobalCacheService(IOptions<Config> options)
			: this(options.Value)
		{ }

		public GlobalCacheService(Config config)
			: this(config.BroadcastIPAddress, config.Port)
		{ }

		public GlobalCacheService(string broadcastIPAddressString, ushort port)
		{
			_port = Guard.Argument(() => port).NotEqual((ushort)0).Value;
			Guard.Argument(() => broadcastIPAddressString)
				.NotNull()
				.NotEmpty()
				.NotWhiteSpace()
				.Matches(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");

			var broadcastIPAddress = IPAddress.Parse(broadcastIPAddressString);

			_socketClient = new Helpers.Networking.Clients.Concrete.SocketClient(1_024, AddressFamily.InterNetwork, ProtocolType.Tcp, SocketType.Stream);
		}
		#endregion constructors

		public async Task ConnectAsync(string hostName)
		{
			Guard.Argument(() => hostName).NotNull().NotEmpty().NotWhiteSpace();

			if (!_cache.TryGetValue(hostName, out var ipAddress))
			{
				ipAddress = await PingAsync(hostName);
				_cache.Add(hostName, ipAddress);
			}

			var endPoint = new IPEndPoint(ipAddress, _port);

			await _socketClient.ConnectAsync(endPoint);
		}

		public async IAsyncEnumerable<string> SendAndReceiveAsync(string message, int count, CancellationToken? cancellationToken = default)
		{
			Guard.Argument(() => message).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => count).Positive();

			while (count-- > 0)
			{
				yield return await _socketClient.SendAndReceiveAsync(message, cancellationToken ?? CancellationToken.None);
				await Task.Delay(millisecondsDelay: 100, cancellationToken ?? CancellationToken.None);
			}
		}

		public async Task<string> ConnectSendReceiveAsync(string hostName, string message)
		{
			Guard.Argument(() => hostName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => message).NotNull().NotEmpty().NotWhiteSpace();

			using var cts = new CancellationTokenSource(millisecondsDelay: 5_000);

			await ConnectAsync(hostName);

			var responses = await SendAndReceiveAsync(message, count: 3, cts.Token).ToListAsync();

			if (responses.Distinct().Count() == 1) return responses.First();

			throw new Exception("unexpected responses") { Data = { [nameof(responses)] = responses, }, };
		}

		private async static Task<IPAddress> PingAsync(string hostName)
		{
			Guard.Argument(() => hostName).NotNull().NotEmpty().NotWhiteSpace();

			var (ipAddress, status) = await Helpers.Networking.NetworkHelpers.PingAsync(hostName);

			if (status == IPStatus.Success)
			{
				return ipAddress;
			}

			throw new Exception($"Pinging {hostName} failed with {nameof(status)}: {status}.")
			{
				Data =
				{
					[nameof(hostName)] = hostName,
					[nameof(status)] = status,
				},
			};
		}

		#region Dispose pattern
		private bool _isDisposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					_socketClient.Dispose();
				}

				_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion Dispose pattern
	}
}
