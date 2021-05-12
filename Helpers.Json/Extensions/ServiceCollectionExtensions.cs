using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection JsonConfig<TOptions>(this IServiceCollection services, IConfiguration config)
			where TOptions : class
		{
			var t = config.JsonConfig<TOptions>();
			var options = Options.Options.Create(t);

			services
				.AddSingleton(options);

			return services;
		}
	}
}
