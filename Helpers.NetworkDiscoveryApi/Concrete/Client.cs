using System.Net.NetworkInformation;

namespace Helpers.NetworkDiscoveryApi.Concrete;

public class Client : Helpers.Web.WebClientBase, IClient
{
	public Client(HttpClient httpClient)
		: base(httpClient)
	{ }

	public async Task<Models.DhcpResponseObject> GetLeaseAsync(PhysicalAddress physicalAddress, CancellationToken? cancellationToken = default)
	{
		var uri = new Uri("/api/router/" + physicalAddress.ToString().ToLowerInvariant(), UriKind.Relative);
		var response = await base.SendAsync<Models.DhcpResponseObject>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		if (response.Exception is not null) throw response.Exception;
		return response.Object!;
	}

	public async IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default)
	{
		var uri = new Uri("/api/router", UriKind.Relative);
		var response = await base.SendAsync<ICollection<Models.DhcpResponseObject>>(HttpMethod.Get, uri, cancellationToken: cancellationToken);
		if (response.Exception is not null) throw response.Exception;

		using var enumerator = response.Object!.GetEnumerator();

		while (enumerator.MoveNext()
			&& cancellationToken?.IsCancellationRequested != true)
		{
			yield return enumerator.Current;
		}
	}
}
