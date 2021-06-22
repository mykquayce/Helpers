using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkService
	{
		Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip);
		Task<Models.RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac);
		Task<Models.RealtimeData> GetRealtimeDataAsync(string alias);
	}
}
