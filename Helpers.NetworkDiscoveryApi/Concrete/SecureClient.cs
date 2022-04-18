using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi.Concrete;

public class SecureClient : Helpers.Identity.SecureWebClientBase, IClient
{
	public SecureClient(HttpClient httpClient, Helpers.Identity.Clients.IIdentityClient identityClient)
		: base(httpClient, identityClient)
	{ }

	public async Task<Models.DhcpResponseObject> GetLeaseAsync(PhysicalAddress physicalAddress, CancellationToken? cancellationToken = default)
	{
		var uri = new Uri("/api/router/" + physicalAddress.ToString().ToLowerInvariant(), UriKind.Relative);
		var (_, _, response) = await base.SendAsync<Models.DhcpResponseObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		return response;
	}

	public async IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default)
	{
		var uri = new Uri("/api/router", UriKind.Relative);
		var (_, _, responses) = await base.SendAsync<ICollection<Models.DhcpResponseObject>>(HttpMethod.Get, uri, cancellationToken: cancellationToken);

		using var enumerator = responses.GetEnumerator();

		while (enumerator.MoveNext()
			&& cancellationToken?.IsCancellationRequested != true)
		{
			yield return enumerator.Current;
		}
	}
}
