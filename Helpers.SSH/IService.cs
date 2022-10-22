using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.SSH;

public interface IService
{
	IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync();
	Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress);
	Task AddBlackholesAsync(IEnumerable<Helpers.Networking.Models.AddressPrefix> subnetAddresses);
	Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix subnetAddress);
	Task DeleteBlackholesAsync(IEnumerable<Helpers.Networking.Models.AddressPrefix> subnetAddresses);
	Task DeleteBlackholesAsync();
	IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync();
	Task<Helpers.Networking.Models.DhcpLease> GetLeaseByIPAddressAsync(IPAddress ipAddress);
	Task<Helpers.Networking.Models.DhcpLease> GetLeaseByPhysicalAddressAsync(PhysicalAddress physicalAddress);
	IAsyncEnumerable<KeyValuePair<PhysicalAddress, IPAddress>> GetArpTableAsync(CancellationToken cancellationToken = default);
}
