using Helpers.TPLink.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkClient
	{
		IAsyncEnumerable<Device> DiscoverAsync();
		Task<RealtimeData> GetRealtimeDataAsync(IPAddress ip);
	}
}
