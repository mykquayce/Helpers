using System.Net;

namespace Helpers.Elgato;

public interface IClient
{
	Task<Models.Generated.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default);
	Task<Models.Generated.LightObject> GetLightAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default);
	Task SetLightAsync(IPAddress ipAddress, Models.Generated.LightObject light, CancellationToken? cancellationToken = default);
}
