using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddAliasResolver(this IServiceCollection services, IConfiguration configuration)
	{
		var config = Helpers.NetworkDiscoveryApi.Config.Defaults;
		configuration.Bind(config);

		return services
			.AddTransient<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()))
			// config
			.AddSingleton(Options.Options.Create(config.Aliases))
			.AddSingleton(Options.Options.Create(config.EndPoints))
			.AddSingleton(Options.Options.Create(config.Identity))
			// identity client
			.AddHttpClient<Helpers.Identity.Clients.IIdentityClient, Helpers.Identity.Clients.Concrete.IdentityClient>((provider, client) =>
			{
				client.BaseAddress = config.EndPoints.IdentityServer;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			// network discovery client
			.AddHttpClient<Helpers.NetworkDiscoveryApi.IClient, Helpers.NetworkDiscoveryApi.Concrete.SecureClient>((provider, client) =>
			{
				client.BaseAddress = config.EndPoints.NetworkDiscoveryApi;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			// network discovery service
			.AddTransient<Helpers.NetworkDiscoveryApi.IService, Helpers.NetworkDiscoveryApi.Concrete.Service>()
			// alias resolver service
			.AddTransient<Helpers.NetworkDiscoveryApi.IAliasResolverService, Helpers.NetworkDiscoveryApi.Concrete.AliasResolverService>();
	}
}
