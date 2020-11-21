using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Helpers.GlobalCache.Clients
{
	public interface IUdpClient : IDisposable
	{
		IAsyncEnumerable<Models.Beacon> DiscoverAsync();
#pragma warning disable CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable
		IAsyncEnumerable<Models.Beacon> DiscoverAsync([EnumeratorCancellation] CancellationToken cancellationToken);
#pragma warning restore CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable
	}
}
