using System;
using System.Threading.Tasks;

namespace Helpers.Elgato.Services
{
	public interface IElgatoService : IDisposable
	{
		Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync();
		Task<Models.MessageObject.LightObject> GetLightAsync();
		Task SetLightAsync(Models.MessageObject.LightObject light);
		Task ToggleLightPowerStateAsync();
	}
}
