using Dawn;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients.Concrete
{
	public class UdpClient : IUdpClient
	{
		public record Config(string BroadcastIPAddress, ushort ReceivePort)
		{
			public Config() : this("239.255.250.250", 9_131) { }
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
			Guard.Argument(broadcastIPAddress).NotNull()
				.Require(ip => !ip.Equals(default));

			Guard.Argument(receivePort).Positive();

			_udpClient = new System.Net.Sockets.UdpClient { ExclusiveAddressUse = false, };
			_udpClient.JoinMulticastGroup(broadcastIPAddress);
			var socket = _udpClient.Client;
			var endPoint = new IPEndPoint(IPAddress.Any, receivePort);
			socket.Bind(endPoint);
		}
		#endregion constructors

		public async IAsyncEnumerable<string> DiscoverAsync()
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);
			await foreach (var s in DiscoverAsync(cts.Token))
			{
				yield return s;
			}
		}

		public async IAsyncEnumerable<string> DiscoverAsync([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var task = await Task.WhenAny(
					_udpClient.ReceiveAsync(),
					Task.Delay(10_000, cancellationToken));

				if (task is not Task<UdpReceiveResult> myTask) continue;

				var result = myTask.Result;
				var s = Encoding.UTF8.GetString(result.Buffer);
				yield return s;
			}
		}

		#region IDisposable implementation
		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					_udpClient?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposed = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~UdpClient()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		void System.IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			System.GC.SuppressFinalize(this);
		}
		#endregion IDisposable implementation
	}
}
