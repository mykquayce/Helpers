using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients
{
	public interface ISocketClient : IDisposable
	{
		Task ConnectAsync(EndPoint endPoint);
		ValueTask<int> SendAsync(string message, CancellationToken? cancellationToken = default);
		ValueTask<int> SendAsync(byte[] bytes, CancellationToken? cancellationToken = default);
		Task<byte[]> ReceiveAsync(CancellationToken? cancellationToken = default);
		Task<string> SendAndReceiveAsync(string message, CancellationToken? cancellationToken = default);
	}
}
