using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.SSH;

public interface IService
{
	IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync(CancellationToken cancellationToken = default);
	Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress, CancellationToken cancellationToken = default);
	Task AddBlackholesAsync(IEnumerable<Helpers.Networking.Models.AddressPrefix> subnetAddresses, CancellationToken cancellationToken = default);
	Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress, CancellationToken cancellationToken = default);
	Task DeleteBlackholesAsync(IEnumerable<Helpers.Networking.Models.AddressPrefix> subnetAddresses, CancellationToken cancellationToken = default);
	Task DeleteBlackholesAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync(CancellationToken cancellationToken = default);
	Task<Helpers.Networking.Models.DhcpLease> GetLeaseByIPAddressAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
	Task<Helpers.Networking.Models.DhcpLease> GetLeaseByPhysicalAddressAsync(PhysicalAddress physicalAddress, CancellationToken cancellationToken = default);
}
