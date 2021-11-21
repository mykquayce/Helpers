using Microsoft.Extensions.Caching.Memory;

namespace Helpers.DockerHub.Tests.Fixtures;

public sealed class AuthorizationClientFixture : IDisposable
{
	private readonly HttpMessageHandler _httpMessageHandler;
	private readonly HttpClient _httpClient;
	private readonly IMemoryCache _memoryCache;

	public AuthorizationClientFixture()
	{
		var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		var username = userSecretsFixture["DockerHub:Username"];
		var password = userSecretsFixture["DockerHub:Password"];

		var config = new Config(username, password, "pihole", "pihole", Config.Scopes.Pull);
		_httpMessageHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(_httpMessageHandler) { BaseAddress = config.AuthApiBaseAddress, };
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		AuthorizationClient = new Concrete.AuthorizationClient(config, _httpClient, _memoryCache);
	}

	public IAuthorizationClient AuthorizationClient { get; }

	public void Dispose()
	{
		_httpClient.Dispose();
		_httpMessageHandler.Dispose();
		_memoryCache.Dispose();
	}
}
