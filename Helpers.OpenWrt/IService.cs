namespace Helpers.OpenWrt;

public interface IService
{
	Task AddBlackholeAsync(Networking.Models.AddressPrefix prefix, CancellationToken cancellationToken = default);
	Task AddBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> prefixes, CancellationToken cancellationToken = default);
	IAsyncEnumerable<Networking.Models.AddressPrefix> GetBlackholesAsync(CancellationToken cancellationToken = default);
	Task DeleteBlackholeAsync(Networking.Models.AddressPrefix blackhole, CancellationToken cancellationToken = default);
}
