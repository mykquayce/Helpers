﻿using System.Runtime.CompilerServices;
using System.Web;

namespace Helpers.NetworkDiscovery.Concrete;

public class Client : Helpers.Identity.SecureWebClientBase, IClient
{
	public Client(HttpClient httpClient, Helpers.Identity.Clients.IIdentityClient identityClient)
		: base(httpClient, identityClient)
	{ }

	public async IAsyncEnumerable<string> GetAliasesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestUri = new Uri("api/router", UriKind.Relative);
		(_, _, var aliases) = await base.SendAsync<IAsyncEnumerable<string>>(HttpMethod.Get, requestUri, cancellationToken: cancellationToken);
		await foreach (var alias in aliases!) { yield return alias; }
	}

	public Task ResetAsync(CancellationToken cancellationToken = default)
	{
		var requestUri = new Uri("api/router/reset", UriKind.Relative);
		return base.SendAsync(HttpMethod.Put, requestUri, cancellationToken: cancellationToken);
	}

	public async Task<Helpers.Networking.Models.DhcpLease> ResolveAsync(object key, CancellationToken cancellationToken = default)
	{
		var requestUri = new Uri("api/router/" + HttpUtility.UrlPathEncode(key.ToString()), UriKind.Relative);
		(_, var status, var lease) = await base.SendAsync<Helpers.Networking.Models.DhcpLease>(HttpMethod.Get, requestUri, cancellationToken: cancellationToken);
		if (status == System.Net.HttpStatusCode.OK)
		{
			return lease!;
		}
		throw new ArgumentOutOfRangeException(nameof(key), key, $"{nameof(key)} {key} not found")
		{
			Data = { [nameof(key)] = key, },
		};
	}
}
