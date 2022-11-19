using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddEnumerableMemoryCache(this IServiceCollection services,
		IOptions<MemoryCacheOptions> optionsAccessor)
	{
		ILoggerFactory loggerFactory;
		{
			var provider = services.BuildServiceProvider();
			loggerFactory = provider.GetService<ILoggerFactory>()
				?? NullLoggerFactory.Instance;
		}
		return services
			.AddEnumerableMemoryCache(optionsAccessor, loggerFactory);
	}

	public static IServiceCollection AddEnumerableMemoryCache(this IServiceCollection services,
		IOptions<MemoryCacheOptions> optionsAccessor,
		ILoggerFactory loggerFactory)
	{
		var cache = new EnumerableMemoryCache(optionsAccessor, loggerFactory);

		return services
			.AddSingleton<IEnumerableMemoryCache>(cache)
			.AddSingleton<IMemoryCache>(cache);
	}
}
