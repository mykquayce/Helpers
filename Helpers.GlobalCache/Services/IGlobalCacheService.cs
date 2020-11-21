using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.GlobalCache.Services
{
	public interface IGlobalCacheService : IDisposable
	{
		IAsyncEnumerable<PhysicalAddress> DiscoverAsync();
		Task SendMessageasync(PhysicalAddress endPoint, string message);
	}
}
