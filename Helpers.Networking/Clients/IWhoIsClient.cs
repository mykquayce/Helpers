using System.Net;

namespace Helpers.Networking.Clients;

public interface IWhoIsClient
{
	IAsyncEnumerable<Models.AddressPrefix> GetIpsAsync(int asn, CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.WhoIsResponse> GetWhoIsDetailsAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
}
