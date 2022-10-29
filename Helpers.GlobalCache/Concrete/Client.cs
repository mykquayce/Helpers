using Dawn;
using Microsoft.Extensions.Options;
using System.Net.Sockets;

namespace Helpers.GlobalCache.Concrete;

public class Client : IClient
{
	private bool _disposed;
	private readonly int _bufferSize;
	private readonly Socket _socket;

	public Client(Socket socket, IOptions<Config> configOptions)
	{
		_socket = socket;
		var config = Guard.Argument(configOptions).NotNull().Wrap(o => o.Value)
			.NotNull().Value;
		(_bufferSize, var hostName, var port) = config;
		Guard.Argument(_bufferSize).Positive();
		Guard.Argument(hostName).NotNull().NotEmpty().NotWhiteSpace();
		_socket.Connect(hostName, port);
	}

	public async Task<ReadOnlyMemory<byte>> SendAsync(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
	{
		Guard.Argument(bytes).NotDefault();
		await _socket.SendAsync(bytes, SocketFlags.None, cancellationToken);
		var buffer = new Memory<byte>(new byte[_bufferSize]);
		var count = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
		return buffer[..count];
	}

	#region IAsyncDisposable implementation
	protected virtual ValueTask DisposeAsync(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				return _socket.DisconnectAsync(reuseSocket: false);
			}

			_disposed = true;
		}

		return ValueTask.CompletedTask;
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsync(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion IAsyncDisposable implementation
}
