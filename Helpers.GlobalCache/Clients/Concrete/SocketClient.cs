using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Clients.Concrete
{
	public class SocketClient : ISocketClient
	{
		private const int _bufferSize = 1_024;
		private readonly Socket _socket;

		public SocketClient()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void Dispose() => _socket.Dispose();

		public ValueTask ConnectAsync(EndPoint endPoint, CancellationToken? cancellationToken = default)
		{
			if (_socket.Connected) return ValueTask.CompletedTask;

			return _socket.ConnectAsync(endPoint, cancellationToken ?? CancellationToken.None);
		}

		public ValueTask<int> SendAsync(string message, CancellationToken? cancellationToken = default)
		{
			var bytes = Encoding.UTF8.GetBytes(message);
			return SendAsync(bytes, cancellationToken ?? CancellationToken.None);
		}

		public ValueTask<int> SendAsync(byte[] bytes, CancellationToken? cancellationToken = default)
			=> _socket.SendAsync(bytes, SocketFlags.None, cancellationToken ?? CancellationToken.None);

		public async IAsyncEnumerable<byte> ReceiveAsync(CancellationToken? cancellationToken = default)
		{
			var count = 0;
			var buffer = new byte[1];
			await using var stream = new NetworkStream(_socket);

			do
			{
				count = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken ?? CancellationToken.None);
				if (count > 0) yield return buffer[0];
			}
			while (count > 0);
		}
	}
}
