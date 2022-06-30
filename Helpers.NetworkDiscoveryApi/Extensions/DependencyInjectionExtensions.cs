using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddNetworkDiscoveryApi(this IServiceCollection services,
		Uri authority, string clientId, string clientSecret, string scope,
		Uri networkDiscoveryApi,
		Helpers.NetworkDiscoveryApi.Aliases aliases)
	{
		var identity = new Helpers.Identity.Config(authority, clientId, clientSecret, scope);
		var endPoints = new Helpers.NetworkDiscoveryApi.EndPoints(authority, networkDiscoveryApi);

		return AddNetworkDiscoveryApi(services, aliases, endPoints, identity);
	}

	public static IServiceCollection AddNetworkDiscoveryApi(this IServiceCollection services, IConfiguration configuration)
	{
		T bind<T>(string section)
			where T : new()
		{
			var t = new T();
			configuration.Bind(t);
			configuration.GetSection(section).Bind(t);
			return t;
		}

		var identity = bind<Helpers.Identity.Config>("identity");
		var endPoints = bind<Helpers.NetworkDiscoveryApi.EndPoints>("endpoints");
		var aliases = Helpers.NetworkDiscoveryApi.Aliases.Bind(configuration.GetSection("aliases"));

		return AddNetworkDiscoveryApi(services, aliases, endPoints, identity);
	}

	public static IServiceCollection AddNetworkDiscoveryApi(this IServiceCollection services,
		Helpers.NetworkDiscoveryApi.Aliases aliases,
		Helpers.NetworkDiscoveryApi.EndPoints endPoints,
		Helpers.Identity.Config identity)
	{
		return services
			.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()))
			// config
			.AddSingleton(Options.Options.Create(aliases))
			.AddSingleton(Options.Options.Create(endPoints))
			.AddSingleton(Options.Options.Create(identity))
			// identity client
			.AddHttpClient<Helpers.Identity.Clients.IIdentityClient, Helpers.Identity.Clients.Concrete.IdentityClient>("IdentityClient", client =>
			{
				client.BaseAddress = endPoints.IdentityServer;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			// network discovery client
			.AddHttpClient<Helpers.NetworkDiscoveryApi.IClient, Helpers.NetworkDiscoveryApi.Concrete.SecureClient>("NetworkDiscoveryClient", client =>
			{
				client.BaseAddress = endPoints.NetworkDiscoveryApi;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			// network discovery service
			.AddTransient<Helpers.NetworkDiscoveryApi.IService, Helpers.NetworkDiscoveryApi.Concrete.Service>();
	}
}
