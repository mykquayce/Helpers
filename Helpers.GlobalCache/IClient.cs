using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.GlobalCache
{
	public interface IClient : IDisposable
	{
		IAsyncEnumerable<Models.Beacon> DiscoverAsync();
		IAsyncEnumerable<Models.Beacon> DiscoverAsync([EnumeratorCancellation] CancellationToken cancellationToken);
		Task<string> SendMessageAsync(EndPoint destination, string message, CancellationToken? cancellationToken = default);
	}
}
