namespace Helpers.NetworkDiscovery;

public interface IClient
{
	IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetAllLeasesAsync(CancellationToken cancellationToken = default);
	Task<HttpResponseMessage> ResetAsync(CancellationToken cancellationToken = default);
	Task<Helpers.Networking.Models.DhcpLease> ResolveAsync(object key, CancellationToken cancellationToken = default);
}
