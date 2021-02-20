using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients
{
	public interface ISocketClient : IDisposable
	{
		ValueTask ConnectAsync(EndPoint endPoint, CancellationToken? cancellationToken = default);
		ValueTask<int> SendAsync(string message, CancellationToken? cancellationToken = default);
		ValueTask<int> SendAsync(byte[] bytes, CancellationToken? cancellationToken = default);
		Task<byte[]> ReceiveAsync(CancellationToken? cancellationToken = default);
	}
}
