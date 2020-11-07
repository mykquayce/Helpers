using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Concrete
{
	public interface ISocketClient : IDisposable
	{
		Task<string> ConnectSendReceiveAsync(EndPoint destination, string message, CancellationToken? cancellationToken = null);
		Task<byte[]> SendReceiveAsync(byte[] messageBytes, CancellationToken? cancellationToken = null);
	}
}
