using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi;

public interface IService
{
	Task<Models.DhcpResponseObject> GetLeaseAsync(PhysicalAddress physicalAddress, CancellationToken? cancellationToken = default);
	Task<Models.DhcpResponseObject> GetLeaseAsync(string alias, CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default);
}
