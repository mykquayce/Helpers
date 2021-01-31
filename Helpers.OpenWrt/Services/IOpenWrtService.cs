using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.OpenWrt.Services
{
	public interface IOpenWrtService
	{
		Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress);
		IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync();
		Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress blackhole);
	}
}
