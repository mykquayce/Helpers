using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.SSH.Services
{
	public interface ISSHService : IDisposable
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
	}
}
