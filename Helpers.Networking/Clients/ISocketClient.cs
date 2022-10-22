using System.Net;

namespace Helpers.Networking.Clients;

public interface ISocketClient : IDisposable
{
	ValueTask ConnectAsync(EndPoint endPoint, CancellationToken cancellationToken = default);
	ValueTask ConnectAsync(IPAddress ipAddress, ushort port, CancellationToken cancellationToken = default);
	ValueTask ConnectAsync(string host, ushort port, CancellationToken cancellationToken = default);
	ValueTask<int> SendAsync(byte[] bytes, CancellationToken cancellationToken = default);
	Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default);
	Task<string> SendAndReceiveAsync(string message, CancellationToken cancellationToken = default);
	Task<string> ConnectSendAndReceive(EndPoint endPoint, string message, CancellationToken cancellationToken = default);
}
