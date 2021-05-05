using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.SSH.Services
{
	public interface ISSHService : IDisposable
	{
		IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync();
		Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress);
		Task AddBlackholesAsync(IEnumerable<Helpers.Networking.Models.SubnetAddress> subnetAddresses);
		Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress);
		Task DeleteBlackholesAsync(IEnumerable<Helpers.Networking.Models.SubnetAddress> subnetAddresses);
		Task DeleteBlackholesAsync();
		IAsyncEnumerable<Helpers.Networking.Models.DhcpEntry> GetDhcpLeasesAsync();
	}
}
