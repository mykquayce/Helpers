using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

namespace Helpers.DockerHub.Concrete;

public class AuthorizationClient : Helpers.Web.WebClientBase, IAuthorizationClient
{
	private const string _cacheKey = "cache-key";
	private readonly IMemoryCache _memoryCache;
	private readonly string _credentials, _repositoryScope;

	public AuthorizationClient(IOptions<Config> options, HttpClient httpClient, IMemoryCache memoryCache)
		: base(httpClient)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;
		_credentials = Guard.Argument(config.Credentials).NotNull().Value;
		_repositoryScope = Guard.Argument(config.RepositoryScope).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
		Organization = config.Organization;
		Repository = config.Repository;
	}

	public string Organization { get; }
	public string Repository { get; }

	public async Task<string> GetTokenAsync(CancellationToken? cancellationToken = default)
	{
		if (_memoryCache.TryGetValue<string>(_cacheKey, out var token))
		{
			return token;
		}

		(token, var expiry) = await GetTokenFromRemoteAsync(cancellationToken);

		// cache
		var millisecondsDelay = (int)(expiry - DateTime.UtcNow).Add(-TimeSpan.FromSeconds(10)).TotalMilliseconds;
		var expirationTokenSource = new CancellationTokenSource(millisecondsDelay);
		var expirationToken = new CancellationChangeToken(expirationTokenSource.Token);
		_memoryCache.Set(_cacheKey, token, expirationToken);

		// return
		return token;
	}

	public async Task<(string token, DateTime expires)> GetTokenFromRemoteAsync(CancellationToken? cancellationToken = default)
	{
		var uriString = "/token?offline_token=1&client_id=shell&service=registry.docker.io&scope=" + _repositoryScope;

		var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString)
		{
			Headers =
			{
				Authorization = new AuthenticationHeaderValue("Basic", _credentials),
			},
		};

		var (_, _, responseObject) = await base.SendAsync<Models.AuthResponseObject>(requestMessage, cancellationToken);
		var (token, _, expiresIn, _) = responseObject;
		var expiry = DateTime.UtcNow.AddSeconds(expiresIn);

		return (token, expiry);
	}
}
