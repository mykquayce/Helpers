namespace Helpers.Identity.Tests.Clients.Concrete;

public class SecureWebClient : SecureWebClientBase, ISecureWebClient
{
	public SecureWebClient(HttpClient apiHttpClient, Identity.Clients.IIdentityClient identityClient)
		: base(apiHttpClient, identityClient)
	{ }

	public async Task<string?> GetStringAsync(Uri relativeUri, CancellationToken cancellationToken = default)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
		(_, _, string? s) = await base.SendAsync<string>(request, cancellationToken);
		return s;
	}
}
