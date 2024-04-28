using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddCachingHandler(this IServiceCollection services, Action<CachingHandler.IConfig> configBuilder)
	{
		var config = new CachingConfig();
		configBuilder(config);

		return services
			.AddSingleton<IOptions<CachingHandler.IConfig>>(Options.Options.Create(config))
			.AddTransient<CachingHandler>();
	}

	public static IHttpClientBuilder AddIdentityServerHandler(this IServiceCollection services, Action<IdentityServerHandler.IConfig> configBuilder)
	{
		var config = new IdentityServerHandlerConfig();
		configBuilder(config);

		return services
			.AddSingleton<IOptions<IdentityServerHandler.IConfig>>(Options.Options.Create(config))
			.AddHttpClient<IdentityServerHandler>(client =>
			{
				client.BaseAddress = config.Authority;
			});
	}

	private record CachingConfig : CachingHandler.IConfig
	{
		public TimeSpan Expiration { get; set; }
	}

	private record IdentityServerHandlerConfig : IdentityServerHandler.IConfig
	{
		public Uri Authority { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Scope { get; set; }
	}
}
