using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi;

public interface IClient
{
	Task<Models.DhcpResponseObject> GetLeaseAsync(PhysicalAddress physicalAddress, CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default);
}
