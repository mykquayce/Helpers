using System.Threading.RateLimiting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
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
