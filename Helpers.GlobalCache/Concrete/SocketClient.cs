using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Concrete
{
	public class SocketClient : ISocketClient
	{
		private const byte _sendCount = 3;
		private readonly Socket _socket;

		public SocketClient()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void Dispose() => _socket.Dispose();

		public async Task<string> ConnectSendReceiveAsync(EndPoint destination, string message, CancellationToken? cancellationToken = default)
		{
			await _socket.ConnectAsync(destination, cancellationToken ?? CancellationToken.None);

			var messageBytes = Encoding.UTF8.GetBytes(message);

			var responseBytes = await SendReceiveNTimesAsync(_sendCount, messageBytes, cancellationToken ?? CancellationToken.None);

			return Encoding.UTF8.GetString(responseBytes);
		}

		public async Task<byte[]> SendReceiveNTimesAsync(int count, byte[] messageBytes, CancellationToken? cancellationToken = default)
		{
			var responses = new byte[count][];

			for (var a = 0; a < count; a++)
			{
				var buffer = new byte[1_024];
				await _socket.SendAsync(messageBytes, SocketFlags.None, cancellationToken ?? CancellationToken.None);
				var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken ?? CancellationToken.None);
				responses[a] = buffer[..bytesRead];
				await Task.Delay(millisecondsDelay: 100);
			}

			return responses[0];
		}

		public async Task<byte[]> SendReceiveAsync(byte[] messageBytes, CancellationToken? cancellationToken = default)
		{
			var buffer = new byte[1_024];
			await _socket.SendAsync(messageBytes, SocketFlags.None, cancellationToken ?? CancellationToken.None);
			var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken ?? CancellationToken.None);
			return buffer[..bytesRead];
		}
	}
}
