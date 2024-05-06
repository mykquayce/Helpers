﻿using Microsoft.Extensions.Options;
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
}
