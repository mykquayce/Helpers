using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Helpers.OpenWrt.Tests.Fixtures;

public sealed class ClientFixture : IDisposable
{
	private readonly HttpClient _httpClient;
	private readonly MemoryCacheFixture _memoryCacheFixture = new();

	public ClientFixture()
	{
		var userSecretsFixture = new XUnitClassFixtures.UserSecretsFixture();
		var config = userSecretsFixture.Configuration
			.GetSection("OpenWrt")
			.Get<Concrete.Client.Config>()
			?? throw new ArgumentNullException("missing config");

		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://" + config.EndPoint), };

		Client = new Concrete.Client(_httpClient, _memoryCacheFixture.MemoryCache, config);
	}
	public IClient Client { get; }

	public void Dispose()
	{
		_memoryCacheFixture.Dispose();
		_httpClient.Dispose();
	}
}
