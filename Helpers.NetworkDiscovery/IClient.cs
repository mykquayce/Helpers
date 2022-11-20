namespace Helpers.NetworkDiscovery;

public interface IClient
{
	IAsyncEnumerable<string> GetAliasesAsync(CancellationToken cancellationToken = default);
	Task ResetAsync(CancellationToken cancellationToken = default);
	Task<Helpers.Networking.Models.DhcpLease> ResolveAsync(object key, CancellationToken cancellationToken = default);
}
