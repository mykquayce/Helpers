using System.Net;

namespace Helpers.Elgato;

public interface IClient
{
	Task<Models.Generated.AccessoryInfoObject> GetAccessoryInfoAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.Generated.LightObject> GetLightsAsync(IPAddress ipAddress, CancellationToken? cancellationToken = default);
	Task SetLightAsync(IPAddress ipAddress, IReadOnlyCollection<Models.Generated.LightObject> lights, CancellationToken? cancellationToken = default);
}
