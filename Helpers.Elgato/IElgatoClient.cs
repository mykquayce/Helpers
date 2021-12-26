using System.Net;

namespace Helpers.Elgato;

public interface IElgatoClient
{
	Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.MessageObject.LightObject> GetLightAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default);
	Task SetLightAsync(IPAddress ipAddress, Models.MessageObject.LightObject light, CancellationToken? cancellationToken = default);
}
