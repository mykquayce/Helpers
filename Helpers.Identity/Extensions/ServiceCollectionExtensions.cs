using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddIdentityClient(this IServiceCollection services, IConfiguration configuration)
	{
		var config = Helpers.Identity.Config.Defaults;
		configuration.Bind(config);
		return services
			.Configure<Helpers.Identity.Config>(configuration)
			.AddIdentityClient(config);
	}

	public static IServiceCollection AddIdentityClient(this IServiceCollection services, IOptions<Helpers.Identity.Config> options)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;

		return services
			.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
			.AddHttpClient<Helpers.Identity.Clients.IIdentityClient, Helpers.Identity.Clients.Concrete.IdentityClient>(client =>
			{
				client.BaseAddress = config.Authority;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services;
	}

	public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, IConfiguration configuration)
	{
		var config = Helpers.Identity.Config.Defaults;
		configuration.Bind(config);
		return services.AddAuthenticationAuthorization(config);
	}

	public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, IOptions<Helpers.Identity.Config> options)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;

		services.AddAuthentication("Bearer")
			.AddJwtBearer("Bearer", options =>
			{
				options.Authority = config.Authority.OriginalString;

				options.TokenValidationParameters = new IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateAudience = false,
				};
			});

		services.AddAuthorization(options =>
		{
			options.AddPolicy("ApiScope", policy =>
			{
				policy.RequireAuthenticatedUser();
				policy.RequireClaim(nameof(config.Scope), config.Scope);
			});
		});

		return services;
	}
}
