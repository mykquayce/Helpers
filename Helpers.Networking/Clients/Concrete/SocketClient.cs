using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;

namespace Helpers.Networking.Clients.Concrete;

public class SocketClient : ISocketClient
{
	#region Config
	public record Config(int BufferSize,
		[property: JsonConverter(typeof(JsonStringEnumConverter))] AddressFamily AddressFamily,
		[property: JsonConverter(typeof(JsonStringEnumConverter))] ProtocolType ProtocolType,
		[property: JsonConverter(typeof(JsonStringEnumConverter))] SocketType SocketType)
	{
		public const int DefaultBufferSize = 1_024;
		public const AddressFamily DefaultAddressFamily = AddressFamily.InterNetwork;
		public const ProtocolType DefaultProtocolType = ProtocolType.Tcp;
		public const SocketType DefaultSocketType = SocketType.Stream;

		public Config() : this(DefaultBufferSize, DefaultAddressFamily, DefaultProtocolType, DefaultSocketType) { }

		public static Config Defaults => new();
	}
	#endregion Config

	private readonly int _bufferSize;
	private readonly Socket _socket;
	private readonly static Encoding _encoding = Encoding.UTF8;

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

	#region Connect
	public ValueTask ConnectAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(endPoint);

		if (_socket.Connected) return ValueTask.CompletedTask;
		return _socket.ConnectAsync(endPoint, cancellationToken);
	}

	public ValueTask ConnectAsync(IPAddress ipAddress, ushort port, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(ipAddress);
		ArgumentOutOfRangeException.ThrowIfZero(port);

		if (_socket.Connected) return ValueTask.CompletedTask;
		return _socket.ConnectAsync(ipAddress, port, cancellationToken);
	}

	public ValueTask ConnectAsync(string host, ushort port, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(host);
		ArgumentOutOfRangeException.ThrowIfZero(port);

		if (_socket.Connected) return ValueTask.CompletedTask;
		return _socket.ConnectAsync(host, port, cancellationToken);
	}
	#endregion Connect

	public ValueTask<int> SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
		=> _socket.SendAsync(bytes, SocketFlags.None, cancellationToken);

	public async Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
	{
		var buffer = new byte[_bufferSize];
		var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
		return buffer[..bytesRead];
	}

	public async Task<string> SendAndReceiveAsync(string message, CancellationToken cancellationToken = default)
	{
		var bytes = _encoding.GetBytes(message);
		await SendAsync(bytes, cancellationToken);
		var responseBytes = await ReceiveAsync(cancellationToken);
		var response = _encoding.GetString(responseBytes);
		return response;
	}

	public async Task<string> ConnectSendAndReceive(EndPoint endPoint, string message, CancellationToken cancellationToken = default)
	{
		await ConnectAsync(endPoint, cancellationToken);
		return await SendAndReceiveAsync(message, cancellationToken);
	}

	#region IDisposable implementation
	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_socket?.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		System.GC.SuppressFinalize(this);
	}
	#endregion IDisposable implementation
}
