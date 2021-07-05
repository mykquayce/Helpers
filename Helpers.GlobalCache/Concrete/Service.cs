using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Concrete
{
	public class Service : IService
	{
		private readonly static IDictionary<string, IPAddress> _cache = new Dictionary<string, IPAddress>(StringComparer.OrdinalIgnoreCase);

		private readonly ushort _broadcastPort;
		private readonly Helpers.Networking.Clients.ISocketClient _socketClient =
			new Helpers.Networking.Clients.Concrete.SocketClient(1_024, AddressFamily.InterNetwork, ProtocolType.Tcp, SocketType.Stream);
		private readonly IClient _client;

		#region constructors
		public Service() : this(Config.Defaults) { }
		public Service(IOptions<Config> options) : this(options.Value) { }
		public Service(Config config) : this(config.BroadcastIPAddress, config.BroadcastPort, config.ReceivePort) { }

		public Service(IPAddress broadcastIPAddress, ushort broadcastPort, ushort receivePort)
		{
			_broadcastPort = Guard.Argument(() => broadcastPort).Positive().Value;
			_client = new Client(broadcastIPAddress, receivePort);
		}
		#endregion constructors

		public async Task ConnectAsync(string uuid)
		{
			Guard.Argument(() => uuid).NotNull().NotEmpty().NotWhiteSpace().Matches("^GlobalCache_[0-9A-F]{12}$");

			if (!_cache.TryGetValue(uuid, out var ipAddress))
			{
				var beacon = await _client.DiscoverAsync()
					.FirstOrDefaultAsync(b => string.Equals(b.Uuid, uuid, StringComparison.OrdinalIgnoreCase));

				Guard.Argument(() => beacon!).NotNull($"{nameof(uuid)} {uuid} not found on network");

				ipAddress = beacon!.IPAddress;
				_cache.Add(uuid, ipAddress);
			}

			var endPoint = new IPEndPoint(ipAddress, _broadcastPort);
			await _socketClient.ConnectAsync(endPoint);
		}

		public async IAsyncEnumerable<string> SendAndReceiveAsync(string message, int count, CancellationToken? cancellationToken = default)
		{
			Guard.Argument(() => message).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => count).Positive();

			while (count-- > 0)
			{
				var task = await Task.WhenAny(
					_socketClient.SendAndReceiveAsync(message, cancellationToken ?? CancellationToken.None),
					Task.Delay(millisecondsDelay: 1_000, cancellationToken ?? CancellationToken.None));

				if (task is Task<string> myTask)
				{
					yield return await myTask;
				}

				await Task.Delay(millisecondsDelay: 100, cancellationToken ?? CancellationToken.None);
			}
		}

		public async Task<string> ConnectSendReceiveAsync(string uuid, string message)
		{
			Guard.Argument(() => uuid).NotNull().NotEmpty().NotWhiteSpace().Matches("^GlobalCache_[0-9A-F]{12}$");
			Guard.Argument(() => message).NotNull().NotEmpty().NotWhiteSpace();

			await ConnectAsync(uuid);

			var responses = await SendAndReceiveAsync(message, count: 3).ToListAsync();

			if (responses.Distinct().Count() == 1) return responses.First();

			throw new Exception("unexpected responses") { Data = { [nameof(responses)] = responses, }, };
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
