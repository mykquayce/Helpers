namespace Helpers.NetworkDiscovery;

public interface IClient
{
	Task<Helpers.Networking.Models.DhcpLease> ResolveAsync(object key, CancellationToken cancellationToken = default);
}
