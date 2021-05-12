using System;
using System.Net;
using System.Threading.Tasks;

namespace Helpers.Elgato
{
	public interface IElgatoClient : IDisposable
	{
		Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(IPEndPoint endPoint);
		Task<Models.MessageObject.LightObject> GetLightAsync(IPEndPoint endPoint);
		Task SetLightAsync(IPEndPoint endPoint, Models.MessageObject.LightObject light);
	}
}
