using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients.Concrete
{
	public class SocketClient : ISocketClient
	{
		#region Config
		public record Config(int BufferSize = 1_024,
			[property: JsonConverter(typeof(JsonStringEnumConverter))] AddressFamily AddressFamily = AddressFamily.InterNetwork,
			[property: JsonConverter(typeof(JsonStringEnumConverter))] ProtocolType ProtocolType = ProtocolType.Tcp,
			[property: JsonConverter(typeof(JsonStringEnumConverter))] SocketType SocketType = SocketType.Stream);
		#endregion Config

		private readonly int _bufferSize;
		private readonly Socket _socket;

		#region Constructors
		public SocketClient(IOptions<Config> options)
			: this(options.Value)
		{ }

		public SocketClient(Config config)
			: this(config.BufferSize, config.AddressFamily, config.ProtocolType, config.SocketType)
		{ }

		public SocketClient(int bufferSize, AddressFamily addressFamily, ProtocolType protocolType, SocketType socketType)
		{
			_bufferSize = bufferSize;
			_socket = new Socket(addressFamily, socketType, protocolType);
		}
		#endregion Constructors

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

		public async Task<byte[]> ReceiveAsync(CancellationToken? cancellationToken = default)
		{
			var buffer = new byte[_bufferSize];
			var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken ?? CancellationToken.None);
			return buffer[..bytesRead];
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
					_socket?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposed = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~SocketClient()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			System.GC.SuppressFinalize(this);
		}
		#endregion IDisposable implementation
	}
}
