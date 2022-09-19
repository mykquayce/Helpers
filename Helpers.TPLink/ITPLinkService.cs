using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkService
	{
		#region getrealtimedata
		Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip);
		Task<Models.RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac);
		Task<Models.RealtimeData> GetRealtimeDataAsync(string alias);
		#endregion getrealtimedata

		#region getsysteminfo
		Task<Models.SystemInfo> GetSystemInfoAsync(IPAddress ip);
		Task<Models.SystemInfo> GetSystemInfoAsync(PhysicalAddress mac);
		Task<Models.SystemInfo> GetSystemInfoAsync(string alias);
		#endregion getsysteminfo

		#region getstate
		Task<bool> GetStateAsync(IPAddress ip);
		Task<bool> GetStateAsync(PhysicalAddress mac);
		Task<bool> GetStateAsync(string alias);
		#endregion getstate

		#region setstate
		Task SetStateAsync(IPAddress ip, bool state);
		Task SetStateAsync(PhysicalAddress mac, bool state);
		Task SetStateAsync(string alias, bool state);
		#endregion setstate
	}
}
