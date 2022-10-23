using Dawn;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Helpers.Identity.Clients.Concrete;

public partial class IdentityClient : IIdentityClient
{
	private const string _cacheKey = "access-token";
	private readonly HttpClient _httpClient;
	private readonly IMemoryCache _memoryCache;
	private readonly string _clientId, _clientSecret, _scope;

	public IdentityClient(
		IOptions<Config> configOptions,
		HttpClient httpClient,
		IMemoryCache memoryCache)
	{
		(_, _clientId, _clientSecret, _scope) = Guard.Argument(configOptions).NotNull().Wrap(o => o.Value)
			.NotNull().Value;
		_httpClient = Guard.Argument(httpClient).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
	}

	public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue<string>(_cacheKey, out var accessToken))
		{
			return accessToken!;
		}

		(accessToken, var expires) = await GetAccessTokenFromRemote(cancellationToken);

		// cache for 90% of its life
		_memoryCache.Set(_cacheKey, accessToken, expires * .9);

		return accessToken;
	}

	private async Task<(string, TimeSpan)> GetAccessTokenFromRemote(CancellationToken cancellationToken = default)
	{
		TokenResponse tokenResponse;
		{
			DiscoveryDocumentResponse disco;
			disco = await _httpClient.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken);
			if (disco.IsError) throw new Exceptions.ProtocolResponseException(disco);
			using var tokenRequest = new ClientCredentialsTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = _clientId,
				ClientSecret = _clientSecret,
				Scope = _scope,
			};
			tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(tokenRequest, cancellationToken);
		}
		if (tokenResponse.IsError) throw new Exceptions.ProtocolResponseException(tokenResponse);
		var expires = TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
		return (tokenResponse.AccessToken, expires);
	}
}
