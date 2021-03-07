using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.SSH.Services
{
	public interface ISSHService : IDisposable
	{
		Task<string> RunCommandAsync(string commandText, int millisecondsTimeout = 5_000);
		IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync();
		Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress);
		Task AddBlackholesAsync(IEnumerable<Helpers.Networking.Models.SubnetAddress> subnetAddresses);
		Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress);
		Task DeleteBlackholesAsec(IEnumerable<Helpers.Networking.Models.SubnetAddress> subnetAddresses);
	}
}
