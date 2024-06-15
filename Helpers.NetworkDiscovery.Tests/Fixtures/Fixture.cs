using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.NetworkDiscovery.Tests.Fixtures;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<XUnitClassFixtures.UserSecretsFixture>()
			.Build();

		var services = new ServiceCollection();

		services
			.AddMemoryCache()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddCachingHandler(c => c.Expiration = TimeSpan.FromHours(.9))
			.AddTransient<LoggingHandler>();

		services
			.AddIdentityServerHandler(b =>
			{
				b.Authority = new Uri("https://identityserver/");
				b.ClientId = "client1";
				b.ClientSecret = "secret1";
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.AddHttpMessageHandler<LoggingHandler>();

		services
			.AddHttpClient<Helpers.NetworkDiscovery.IClient, Helpers.NetworkDiscovery.Concrete.Client>(name: "NetworkDiscoveryClient", (provider, httpClient) =>
			{
				var baseAddress = configuration.GetSection("networkdiscovery").GetValue<Uri>("baseaddress");
				httpClient.BaseAddress = baseAddress;
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<IdentityServerHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.AddHttpMessageHandler<LoggingHandler>();

		services
			.AddTransient<PhysicalAddressResolver>();

		services
			.AddHttpClient<TestClient>()
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<PhysicalAddressResolver>();

		_serviceProvider = services.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
		TestClient = _serviceProvider.GetRequiredService<TestClient>();
	}

	public IClient Client { get; }
	public TestClient TestClient { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
