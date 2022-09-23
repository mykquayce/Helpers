using Helpers.TPLink.Models;
using System.Net;

namespace Helpers.TPLink;

public interface ITPLinkClient
{
	IAsyncEnumerable<Device> DiscoverAsync();
	Task<RealtimeData> GetRealtimeDataAsync(IPAddress ip);
	Task<SystemInfo> GetSystemInfoAsync(IPAddress ip);
	Task<bool> GetStateAsync(IPAddress ip);
	Task SetStateAsync(IPAddress ip, bool state);
}
