using Dawn;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace Helpers.Identity.Clients.Concrete;

public class IdentityClient(IOptions<Config> config, HttpClient httpClient) : IIdentityClient
{
	private readonly string _clientId = Guard.Argument(config).NotNull().Wrap(o => o.Value)
		.NotNull().Wrap(c => c.ClientId).NotEmpty().Value;
	private readonly string _clientSecret = Guard.Argument(config.Value.ClientSecret).NotEmpty().Value;
	private readonly string _scope = Guard.Argument(config.Value.Scope).NotEmpty().Value;

	public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		TokenResponse tokenResponse;
		{
			DiscoveryDocumentResponse disco;
			disco = await httpClient.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken);
			if (disco.IsError) throw new ProtocolResponseException(disco);
			using var tokenRequest = new ClientCredentialsTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = _clientId,
				ClientSecret = _clientSecret,
				Scope = _scope,
			};
			tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest, cancellationToken);
		}
		if (tokenResponse.IsError) throw new ProtocolResponseException(tokenResponse);
		return tokenResponse.AccessToken!;
	}
}
