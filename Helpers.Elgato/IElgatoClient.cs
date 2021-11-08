using System;
using System.Net;
using System.Threading.Tasks;

namespace Helpers.Elgato
{
	public interface IElgatoClient
	{
		Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress);
		Task<Models.MessageObject.LightObject> GetLightAsync(IPAddress ipAddress);
		Task SetLightAsync(IPAddress ipAddress, Models.MessageObject.LightObject light);
		Task ToggleLightAsync(IPAddress ipAddress);
	}
}
