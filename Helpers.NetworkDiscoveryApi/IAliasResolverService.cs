using System.Net;

namespace Helpers.NetworkDiscoveryApi;

public interface IAliasResolverService
{
	Task<IPAddress> ResolveAsync(string alias, CancellationToken? cancellationToken = default);
}
