using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Identity.Tests;

public sealed class Fixture : IDisposable
{
	private static readonly Uri _baseAddress = new("https://identityserver/");
	private const string _clientId = "client", _clientSecret = "secret", _scope = "api1";

	private readonly IServiceProvider _provider;

	public Fixture()
	{
		var initialData = new Dictionary<string, string?>
		{
			["clientid"] = _clientId,
			["clientsecret"] = _clientSecret,
			["scope"] = _scope,
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(initialData)
			.Build();

		_provider = new ServiceCollection()
			.AddMemoryCache()
			.Configure<Config>(configuration)
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddTransient<CachingHandler>()
			.AddHttpClient<Clients.IIdentityClient, Clients.Concrete.IdentityClient>(c => c.BaseAddress = _baseAddress)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<CachingHandler>()
				.Services
			.BuildServiceProvider();

		IdentityClient = _provider.GetRequiredService<Clients.IIdentityClient>();
	}

	public Clients.IIdentityClient IdentityClient { get; }

	public void Dispose() => (_provider as ServiceProvider)?.Dispose();
}
