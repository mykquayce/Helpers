﻿using Dawn;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace Helpers.GlobalCache.Concrete;

public class Service : IService
{
	private readonly static IDictionary<string, IPAddress> _cache = new Dictionary<string, IPAddress>(StringComparer.OrdinalIgnoreCase);

	private readonly ushort _broadcastPort;
	private readonly Helpers.Networking.Clients.ISocketClient _socketClient =
		new Helpers.Networking.Clients.Concrete.SocketClient(1_024, AddressFamily.InterNetwork, ProtocolType.Tcp, SocketType.Stream);
	private readonly IClient _client;

	#region constructors
	public Service(IOptions<Config> options) : this(options.Value) { }
	public Service(Config config)
	{
		_broadcastPort = Guard.Argument(config).NotNull().Wrap(c => c.BroadcastPort).Positive().Value;
		_client = new Client(config);
	}
	#endregion constructors

	#region connect
	public ValueTask ConnectAsync(string uuidOrHostName, CancellationToken cancellationToken = default)
	{
		Guard.Argument(uuidOrHostName).NotNull().NotEmpty().NotWhiteSpace();

		if (DawnGuardExtensions.UuidRegex.IsMatch(uuidOrHostName))
		{
			return ConnectWithUuidAsync(uuidOrHostName, cancellationToken);
		}

		return ConnectWithHostNameAsync(uuidOrHostName, cancellationToken);
	}

	public ValueTask ConnectAsync(IPAddress ipAddress, CancellationToken cancellationToken = default)
	{
		Guard.Argument(ipAddress).NotNull();
		var endPoint = new IPEndPoint(ipAddress, _broadcastPort);
		return _socketClient.ConnectAsync(endPoint, cancellationToken);
	}

	public async ValueTask ConnectWithUuidAsync(string uuid, CancellationToken cancellationToken = default)
	{
		Guard.Argument(uuid).IsGlobalCacheUuid();

		if (!_cache.TryGetValue(uuid, out var ipAddress))
		{
			var beacon = await _client.DiscoverAsync()
				.FirstOrDefaultAsync(b => string.Equals(b.Uuid, uuid, StringComparison.OrdinalIgnoreCase), cancellationToken);

			Guard.Argument(beacon!).NotNull($"{nameof(uuid)} {uuid} not found on network");

			ipAddress = beacon!.IPAddress;
			_cache.Add(uuid, ipAddress);
		}

		await ConnectAsync(ipAddress, cancellationToken);
	}

	public ValueTask ConnectWithHostNameAsync(string hostName, CancellationToken cancellationToken = default)
	{
		Guard.Argument(hostName).NotNull().NotEmpty().NotWhiteSpace();
		return _socketClient.ConnectAsync(hostName, _broadcastPort, cancellationToken);
	}
	#endregion connect

	public Task<string> SendAndReceiveAsync(string message, CancellationToken cancellationToken = default)
	{
		Guard.Argument(message).NotNull().NotEmpty().NotWhiteSpace();
		return _socketClient.SendAndReceiveAsync(message, cancellationToken);
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
