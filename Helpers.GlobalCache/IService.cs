using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache
{
	public interface IService : IDisposable
	{
		Task ConnectAsync(string uuid);
		Task ConnectAsync(IPAddress ipAddress);
		IAsyncEnumerable<string> SendAndReceiveAsync(string message, int count, CancellationToken? cancellationToken = default);
		Task<string> ConnectSendReceiveAsync(string uuid, string message);
		Task<string> ConnectSendReceiveAsync(IPAddress ipAddress, string message);
	}
}
