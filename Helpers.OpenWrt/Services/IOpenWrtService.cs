using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.OpenWrt.Services
{
	public interface IOpenWrtService : IDisposable
	{
		Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress);
		Task AddBlackholesAsync(IEnumerable<Helpers.Networking.Models.SubnetAddress> subnetAddresses);
		IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync();
		Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress blackhole);
	}
}
