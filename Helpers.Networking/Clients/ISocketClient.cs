using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients
{
	public interface ISocketClient : IDisposable
	{
		Task ConnectAsync(EndPoint endPoint);
		Task ConnectAsync(IPAddress ipAddress, ushort port);
		Task ConnectAsync(string host, ushort port);
		Task<int> SendAsync(byte[] bytes, CancellationToken? cancellationToken = default);
		Task<byte[]> ReceiveAsync(CancellationToken? cancellationToken = default);
		Task<string> SendAndReceiveAsync(string message, CancellationToken? cancellationToken = default);
		Task<string> ConnectSendAndReceive(EndPoint endPoint, string message, CancellationToken? cancellationToken = default);
	}
}
