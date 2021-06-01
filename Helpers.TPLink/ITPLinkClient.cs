using Helpers.TPLink.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkClient
	{
		IAsyncEnumerable<Device> DiscoverAsync();
		Task<RealtimeData> GetRealtimeDataAsync(Device device);
		Task<RealtimeData> GetRealtimeDataAsync(IPAddress ip);
		Task<RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac);
		Task<RealtimeData> GetRealtimeDataAsync(string alias);
	}
}
