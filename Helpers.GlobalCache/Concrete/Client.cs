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
		ArgumentNullException.ThrowIfNull(configOptions);
		ArgumentNullException.ThrowIfNull(configOptions.Value);
		ArgumentNullException.ThrowIfNull(configOptions.Value.Value);

		_socket = socket;
		(_bufferSize, var hostName, var port) = configOptions.Value.Value;

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_bufferSize);
		ArgumentException.ThrowIfNullOrWhiteSpace(hostName);

		_socket.Connect(hostName, port);
	}

	public async Task<ReadOnlyMemory<byte>> SendAsync(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(bytes);
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
