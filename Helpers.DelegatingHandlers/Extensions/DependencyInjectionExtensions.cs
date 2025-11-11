using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

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
		var config = IdentityServerHandlerConfig.Empty;
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
		public required Uri Authority { get; set; }
		public required string ClientId { get; set; }
		public required string ClientSecret { get; set; }

		public static IdentityServerHandler.IConfig Empty => new IdentityServerHandlerConfig { Authority = default!, ClientId = default!, ClientSecret = default!, };
	}

	public static IServiceCollection AddRateLimitHandler(this IServiceCollection services, TimeSpan replenishmentPeriod, int tokenLimit, int tokensPerPeriod)
	{
		var options = new TokenBucketRateLimiterOptions { ReplenishmentPeriod = replenishmentPeriod, TokenLimit = tokenLimit, TokensPerPeriod = tokensPerPeriod, };

		return services
			.AddSingleton(_ => options)
			.AddSingleton<RateLimiter, TokenBucketRateLimiter>()
			.AddTransient<RateLimitHandler>();
	}

	public static IServiceCollection AddUserAgentHandler(this IServiceCollection services, string userAgent)
	{
		ArgumentException.ThrowIfNullOrEmpty(userAgent);
		var handler = new UserAgentHandler(userAgent);
		return services.AddTransient(_ => handler);
	}

	#region retry handler
	public static IServiceCollection AddRetryHandler(this IServiceCollection services, IConfiguration configuration)
	{
		var config = new RetryHandler.Config();
		configuration.Bind(config);
		return services.AddRetryHandler(config);
	}

	public static IServiceCollection AddRetryHandler(this IServiceCollection services, Action<RetryHandler.Config> builder)
	{
		var config = new RetryHandler.Config();
		builder(config);
		return services.AddRetryHandler(config);
	}

	public static IServiceCollection AddRetryHandler(this IServiceCollection services, RetryHandler.Config config)
	{
		ArgumentNullException.ThrowIfNull(config);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(config.Count);
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(config.Pause, TimeSpan.Zero);

		return services
			.AddTransient(_ => Options.Options.Create(config))
			.AddTransient<RetryHandler>();
	}
	#endregion retry handler
}
