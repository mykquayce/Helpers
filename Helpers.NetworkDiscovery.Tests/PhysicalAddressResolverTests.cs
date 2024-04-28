using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.NetworkDiscovery.Tests;

public class PhysicalAddressResolverTests
{
	[Theory]
	[InlineData("http://3c6a9d14d765:9123/", "elgato/accessory-info")]
	public async Task Test1(string baseAddress, string requestUri)
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<XUnitClassFixtures.UserSecretsFixture>()
			.Build();

		using var provider = new ServiceCollection()
			.Configure<Helpers.NetworkDiscovery.Config>(configuration.GetSection("networkdiscovery"))
			.Configure<IdentityServerDelegatingHandler.Config>(configuration.GetSection("identity"))
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddHttpClient<IdentityServerDelegatingHandler>((provider, client) =>
			{
				var config = provider.GetRequiredService<IOptions<IdentityServerDelegatingHandler.Config>>().Value;
				client.BaseAddress = config.Authority;

			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.Services
			.AddHttpClient<IClient, Concrete.Client>(name: "NetworkDiscoveryClient", (provider, client) =>
			{
				var config = provider.GetRequiredService<IOptions<Helpers.NetworkDiscovery.Config>>().Value;
				client.BaseAddress = config.BaseAddress;
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<IdentityServerDelegatingHandler>()
				.Services
			.AddTransient<PhysicalAddressResolver>()
			.AddHttpClient<Client>(name: "test-client", client =>
			{
				client.BaseAddress = new Uri(baseAddress);
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<PhysicalAddressResolver>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<Client>();

		var response = await sut.GetAsync(requestUri);
		var content = await response.Content.ReadAsStringAsync();

		Assert.True(response.IsSuccessStatusCode, response.StatusCode + " " + content);
		Assert.NotEmpty(content);
		Assert.StartsWith("{", content);
		Assert.NotEqual("{}", content);
	}

	private class Client(HttpClient httpClient)
	{
		public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
			=> httpClient.GetAsync(requestUri, cancellationToken);
	}
}
