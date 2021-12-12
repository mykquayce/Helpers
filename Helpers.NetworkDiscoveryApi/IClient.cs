namespace Helpers.NetworkDiscoveryApi;

public interface IClient
{
	IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default);
}
