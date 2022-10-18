using System.Net;

namespace Helpers.PhilipsHue;

public interface IDiscoveryClient
{
	Task<IPAddress> GetBridgeIPAddressAsync(CancellationToken? cancellationToken = null);
}
