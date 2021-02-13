using Dawn;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Clients.Concrete
{
	public class UdpClient : IUdpClient
	{
		public record Config
		{
			public string BroadcastIPAddress { get; init; } = "239.255.250.250";
			public ushort ReceivePort { get; init; } = 9_131;
		}

		private readonly System.Net.Sockets.UdpClient _udpClient;

		#region constructors
		public UdpClient(IOptions<Config> options)
			: this(options.Value)
		{ }

		public UdpClient(Config config)
			: this(IPAddress.Parse(config.BroadcastIPAddress), config.ReceivePort)
		{ }

		public UdpClient(IPAddress broadcastIPAddress, ushort receivePort)
		{
			Guard.Argument(() => broadcastIPAddress).NotNull()
				.Require(ip => !ip.Equals(default));

			Guard.Argument(() => receivePort).Positive();

			_udpClient = new System.Net.Sockets.UdpClient { ExclusiveAddressUse = false, };
			_udpClient.JoinMulticastGroup(broadcastIPAddress);
			var socket = _udpClient.Client;
			var endPoint = new IPEndPoint(IPAddress.Any, receivePort);
			socket.Bind(endPoint);
		}

		public void Dispose() => _udpClient?.Dispose();
		#endregion constructors

		public async IAsyncEnumerable<Models.Beacon> DiscoverAsync()
		{
			string? uuid = default;

			using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);

			await foreach (var beacon in DiscoverAsync(cts.Token))
			{
				if (uuid != beacon.Uuid)
				{
					yield return beacon;
				}

				uuid = beacon.Uuid;
			}
		}

		public async IAsyncEnumerable<Models.Beacon> DiscoverAsync([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var task = await Task.WhenAny(
					_udpClient.ReceiveAsync(),
					Task.Delay(10_000, cancellationToken));

				if (task is not Task<UdpReceiveResult> myTask) continue;

				var result = myTask.Result;
				var s = Encoding.UTF8.GetString(result.Buffer);
				var beacon = Models.Beacon.Parse(s);
				yield return beacon;
			}
		}
	}
}
