using IdentityModel.Client;

namespace Helpers.Identity;

public abstract class SecureWebClientBase : Helpers.Web.WebClientBase
{
	private readonly Clients.IIdentityClient _identityClient;

	protected SecureWebClientBase(
		HttpClient httpClient,
		Clients.IIdentityClient identityClient)
		: base(httpClient)
	{
		_identityClient = identityClient;
	}

	protected override async Task<HttpResponseMessage> InvokeAsync(HttpRequestMessage request, CancellationToken? cancellationToken = default)
	{
		var accessToken = await _identityClient.GetAccessTokenAsync(cancellationToken);

		request.SetBearerToken(accessToken);

		return await base.InvokeAsync(request, cancellationToken);
	}
}
