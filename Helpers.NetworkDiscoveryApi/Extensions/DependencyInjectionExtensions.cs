using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	/// <summary>
	/// adds the alias resolver and its dependencies, i.e.,
	/// <see cref="IMemoryCache">IMemoryCache</see>,
	/// <see cref="Helpers.Identity.Clients.IIdentityClient">IIdentityClient</see>,
	/// <see cref="Helpers.NetworkDiscoveryApi.IClient">IClient</see>,
	/// <see cref="Helpers.NetworkDiscoveryApi.IService">IService</see>,
	/// <see cref="Helpers.NetworkDiscoveryApi.IAliasResolverService">IAliasResolverService</see>
	/// </summary>
	/// <remarks>
	/// configuration comes from:
	/// EndPoints:IdentityServer, EndPoints:NetworkDiscoveryApi, Identity:Authority,
	/// Identity:ClientId, Identity:ClientSecret, and Identity:Scope.
	/// MAC addresses under e.g., Aliases:keylight and Aliases:lightstrip
	/// </remarks>
	/// <param name="services"><see cref="IServiceCollection">IServiceCollection</see></param>
	/// <param name="configuration"><see cref="IConfiguration">IConfiguration</see></param>
	/// <returns><see cref="IServiceCollection">IServiceCollection</see></returns>
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
			.AddHttpClient<Helpers.Identity.Clients.IIdentityClient, Helpers.Identity.Clients.Concrete.IdentityClient>("IdentityClient", (provider, client) =>
			{
				client.BaseAddress = config.EndPoints.IdentityServer;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			// network discovery client
			.AddHttpClient<Helpers.NetworkDiscoveryApi.IClient, Helpers.NetworkDiscoveryApi.Concrete.SecureClient>("NetworkDiscoveryClient", (provider, client) =>
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
