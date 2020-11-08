using System;
using System.Threading.Tasks;

namespace Helpers.Elgato.Clients
{
	public interface IElgatoClient : IDisposable
	{
		Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync();
		Task<Models.MessageObject.LightObject> GetLightAsync();
		Task SetLightAsync(Models.MessageObject.LightObject light);
	}
}
