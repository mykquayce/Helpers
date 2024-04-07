using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace System.Net.Http;

public class IdentityServerDelegatingHandler(HttpClient httpClient, IOptions<IdentityServerDelegatingHandler.Config> config)
	: DelegatingHandler
{
	private readonly Config _config = config.Value;

	public record Config(Uri Authority, string ClientId, string ClientSecret, string Scope)
	{
		public Config() : this(default!, default!, default!, default!) { }
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (request.Headers.Authorization != null)
		{
			return await base.SendAsync(request, cancellationToken);
		}

		var token = await GetAccessTokenAsync(cancellationToken);

		request.Headers.Authorization = new Headers.AuthenticationHeaderValue("Bearer", token);

		return await base.SendAsync(request, cancellationToken);
	}

	public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		TokenResponse tokenResponse;
		{
			var disco = await httpClient.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken);
			if (disco.IsError) @throw(disco);
			using var tokenRequest = new ClientCredentialsTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = _config.ClientId,
				ClientSecret = _config.ClientSecret,
				Scope = _config.Scope,
			};
			tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest, cancellationToken);
		}
		if (tokenResponse.IsError) @throw(tokenResponse);
		return tokenResponse.AccessToken!;

		static void @throw(ProtocolResponse response)
		{
			throw new InvalidOperationException(message: $"error {response.ErrorType}: ({response.Error})")
			{
				Data =
				{
					[nameof(response.ErrorType)] = response.ErrorType,
					[nameof(response.Error)] = response.Error,
					[nameof(response.HttpErrorReason)] = response.HttpErrorReason,
					[nameof(response.Raw)] = response.Raw,
				},
			};
		}
	}
}
