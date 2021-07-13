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
		Task<string> SendAndReceiveAsync(string message, CancellationToken? cancellationToken = default);
	}
}
