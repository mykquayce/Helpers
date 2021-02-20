using System;
using System.Collections.Generic;

namespace Helpers.GlobalCache.Clients
{
	public interface IDiscoveryClient : IDisposable
	{
		IAsyncEnumerable<Models.Beacon> DiscoverAsync();
	}
}
