using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddNetworkDiscovery(
		this IServiceCollection services,
		Uri baseAddress,
		Uri authority, string clientId, string clientSecret, string scope)
	{
		var identityConfig = new Helpers.Identity.Config(authority, clientId, clientSecret, scope);
		var config = new Helpers.NetworkDiscovery.Config(baseAddress);

		return services
			.AddNetworkDiscovery(identityConfig, config);
	}

	public static IServiceCollection AddNetworkDiscovery(this IServiceCollection services, IConfiguration identityConfiguration, IConfiguration configuration)
	{
		return services
			.Configure<Helpers.Identity.Config>(identityConfiguration)
			.Configure<Helpers.NetworkDiscovery.Config>(configuration)
			.AddNetworkDiscovery();
	}

	public static IServiceCollection AddNetworkDiscovery(this IServiceCollection services, Helpers.Identity.Config identityConfig, Helpers.NetworkDiscovery.Config config)
	{
		return services
			.AddSingleton(Options.Options.Create(identityConfig))
			.AddSingleton(Options.Options.Create(config))
			.AddNetworkDiscovery();
	}

	public static IServiceCollection AddNetworkDiscovery(this IServiceCollection services)
	{
		return services
			.AddIdentityClient()
			.AddHttpClient<Helpers.NetworkDiscovery.IClient, Helpers.NetworkDiscovery.Concrete.Client>(name: "NetworkDiscoveryClient", (provider, httpClient) =>
			{
				var config = provider.GetRequiredService<IOptions<Helpers.NetworkDiscovery.Config>>().Value.Value;
				httpClient.BaseAddress = config.BaseAddress;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services;
	}
}
