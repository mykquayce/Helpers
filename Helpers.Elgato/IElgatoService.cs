using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.Elgato
{
	public interface IElgatoService : IDisposable
	{
		Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(PhysicalAddress physicalAddress);
		Task<Models.MessageObject.LightObject> GetLightAsync(PhysicalAddress physicalAddress);
		Task SetLightAsync(PhysicalAddress physicalAddress, Models.MessageObject.LightObject light);
		Task ToggleLightPowerStateAsync(PhysicalAddress physicalAddress);
	}
}
