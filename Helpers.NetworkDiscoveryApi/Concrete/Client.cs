﻿namespace Helpers.NetworkDiscoveryApi.Concrete;

public class Client : Helpers.Web.WebClientBase, IClient
{
	public Client(HttpClient httpClient)
		: base(httpClient)
	{ }

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
