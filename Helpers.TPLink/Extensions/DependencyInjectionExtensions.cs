using Helpers.TPLink;
using Helpers.TPLink.Concrete;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddTPLink(this IServiceCollection services, ushort port)
	{
		var config = new Config(port);
		return services.AddTPLink(config);
	}

	public static IServiceCollection AddTPLink(this IServiceCollection services, Config config)
	{
		return services
			.AddSingleton<IOptions<Config>>(config)
			.TryAddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
			.AddTransient<ITPLinkClient, TPLinkClient>()
			.AddTransient<ITPLinkService, TPLinkService>();
	}

	public static IServiceCollection TryAddSingleton<TService>(
		this IServiceCollection collection, TService instance)
		where TService : class
	{
		ServiceCollectionDescriptorExtensions.TryAddSingleton<TService>(collection, instance);
		return collection;
	}
}
