using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Concrete
{
	public class Client : IClient
	{
		public record Config
		{
			public IPAddress BroadcastIPAddress { get; init; } = IPAddress.Parse("239.255.250.250");
			public ushort ReceivePort { get; init; } = 9_131;
		}

		private readonly UdpClient _udpClient;

		#region constructors
		public Client(IOptions<Config> options)
			: this(options.Value)
		{ }

		public Client(Config config)
			: this(config.BroadcastIPAddress, config.ReceivePort)
		{ }

		public Client(IPAddress broadcastIPAddress, ushort receivePort)
		{
			Guard.Argument(() => broadcastIPAddress).NotNull()
				.Require(ip => !ip.Equals(default));

			Guard.Argument(() => receivePort).Positive();

			_udpClient = new UdpClient { ExclusiveAddressUse = false, };
			_udpClient.JoinMulticastGroup(broadcastIPAddress);
			var socket = _udpClient.Client;
			socket.Bind(IPEndPoint.Parse("0.0.0.0:" + receivePort));
		}

		public void Dispose() => _udpClient?.Dispose();
		#endregion constructors

		public async IAsyncEnumerable<Models.Beacon> DiscoverAsync()
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);

			await foreach (var beacon in DiscoverAsync(cts.Token))
			{
				yield return beacon;
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

		public async Task<string> SendMessageAsync(EndPoint destination, string message, CancellationToken? cancellationToken = default)
		{
			using var socket = new SocketClient();

			var response = await socket.ConnectSendReceiveAsync(destination, message, cancellationToken ?? CancellationToken.None);

			if (response.StartsWith("completeir", StringComparison.InvariantCultureIgnoreCase))
			{
				return response;
			}

			throw new Exception(response);
		}
	}
}
