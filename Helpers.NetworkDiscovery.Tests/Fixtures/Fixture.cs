using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.NetworkDiscovery.Tests.Fixtures;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<XUnitClassFixtures.UserSecretsFixture>()
			.Build();

		_serviceProvider = new ServiceCollection()
			.AddMemoryCache()
			.Configure<Helpers.NetworkDiscovery.Config>(configuration.GetSection("networkdiscovery"))
			.Configure<IdentityServerDelegatingHandler.Config>(configuration.GetSection("identity"))
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddTransient<CachingHandler>()
			.AddHttpClient<IdentityServerDelegatingHandler>((provider, httpClient) =>
			{
				var config = provider.GetRequiredService<IOptions<IdentityServerDelegatingHandler.Config>>().Value;
				httpClient.BaseAddress = config.Authority;
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.AddHttpMessageHandler<LoggingHandler>()
				.Services
			.AddTransient<LoggingHandler>()
			.AddHttpClient<Helpers.NetworkDiscovery.IClient, Helpers.NetworkDiscovery.Concrete.Client>(name: "NetworkDiscoveryClient", (provider, httpClient) =>
			{
				var config = provider.GetRequiredService<IOptions<Helpers.NetworkDiscovery.Config>>().Value;
				httpClient.BaseAddress = config.BaseAddress;
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<IdentityServerDelegatingHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.AddHttpMessageHandler<LoggingHandler>()
				.Services
			.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
	}

	public IClient Client { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
