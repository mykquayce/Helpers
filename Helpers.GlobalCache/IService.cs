using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache
{
	public interface IService : IDisposable
	{
		Task ConnectAsync(string uuid);
		IAsyncEnumerable<string> SendAndReceiveAsync(string message, int count, CancellationToken? cancellationToken = default);
		Task<string> ConnectSendReceiveAsync(string uuid, string message);
	}
}
