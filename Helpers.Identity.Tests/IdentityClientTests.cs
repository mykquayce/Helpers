using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;

namespace Helpers.Identity.Tests;

public class IdentityClientTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private readonly Config _config;

	public IdentityClientTests(Fixtures.ConfigurationFixture fixture)
	{
		_config = fixture.Config;
	}

	[Theory]
	[InlineData(10)]
	public async Task GetAccessTokenTests(int count)
	{
		ICollection<string> accessTokens = new List<string>();

		{
			using var httpClient = new HttpClient
			{
				BaseAddress = _config.Authority,
			};
			using var memoryCache = new MemoryCache(new MemoryCacheOptions());
			Identity.Clients.IIdentityClient client = new Helpers.Identity.Clients.Concrete.IdentityClient(_config, httpClient, memoryCache);

			while (count-- > 0)
			{
				var accessToken = await client.GetAccessTokenAsync();
				accessTokens.Add(accessToken);
			}
		}

		Assert.NotEmpty(accessTokens);
		Assert.All(accessTokens, Assert.NotNull);
		Assert.All(accessTokens, Assert.NotEmpty);
		Assert.All(accessTokens, s => Assert.Matches(@"^[0-9A-Za-z]{160}\.[0-9A-Za-z]{200,300}\.[\-0-9A-Z_a-z]{300,400}$", s));
	}

	[Fact]
	public async Task UseAccessTokenTests()
	{
		string response;
		{
			string accessToken;
			{
				using var httpClientHander = new HttpClientHandler { AllowAutoRedirect = false, };
				using var httpClient = new HttpClient(httpClientHander) { BaseAddress = _config.Authority, };
				using var memoryCache = new MemoryCache(new MemoryCacheOptions());
				Identity.Clients.IIdentityClient client = new Helpers.Identity.Clients.Concrete.IdentityClient(_config, httpClient, memoryCache);
				accessToken = await client.GetAccessTokenAsync();
			}
			{
				using var factory = new WebApplicationFactory<Helpers.Identity.Tests.TestApi.Program>();
				using var httpClient = factory.CreateClient();
				httpClient.SetBearerToken(accessToken);

				response = await httpClient.GetStringAsync("weatherforecast");
			}
		}
		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("[", response);
	}
}
