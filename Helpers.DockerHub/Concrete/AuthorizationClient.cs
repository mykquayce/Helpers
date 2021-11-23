using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

namespace Helpers.DockerHub.Concrete;

public class AuthorizationClient : Helpers.Web.WebClientBase, IAuthorizationClient
{
	private readonly IMemoryCache _memoryCache;
	private readonly string _credentials;

	public AuthorizationClient(IOptions<Config> options, HttpClient httpClient, IMemoryCache memoryCache)
		: base(httpClient)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;
		_credentials = Guard.Argument(config.Credentials).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
	}

	public async Task<string> GetTokenAsync(string organization, string repository, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(organization).IsTagName();
		Guard.Argument(repository).IsTagName();

		var cacheKey = BuildCacheKey(organization, repository);

		if (_memoryCache.TryGetValue<string>(cacheKey, out var token))
		{
			return token;
		}

		(token, var expiry) = await GetTokenFromRemoteAsync(organization, repository, cancellationToken);

		// cache
		var timeoutDelay = (int)(expiry - DateTime.UtcNow).Add(-TimeSpan.FromSeconds(10)).TotalMilliseconds;
		var expirationTokenSource = new CancellationTokenSource(timeoutDelay);
		var expirationToken = new CancellationChangeToken(expirationTokenSource.Token);
		_memoryCache.Set(cacheKey, token, expirationToken);

		// return
		return token;
	}

	public async Task<(string token, DateTime expires)> GetTokenFromRemoteAsync(string organization, string repository, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(organization).IsTagName();
		Guard.Argument(repository).IsTagName();
		
		var repositoryScope = $"repository:{organization}/{repository}:pull";

		var uriString = "/token?offline_token=1&client_id=shell&service=registry.docker.io&scope=" + repositoryScope;

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

	private static string BuildCacheKey(params string[] values) => string.Join('/', values).ToLowerInvariant();
}
