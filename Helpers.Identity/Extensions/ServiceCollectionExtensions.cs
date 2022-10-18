using Dawn;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	#region AddIdentityClient

	public static IServiceCollection AddIdentityClient(this IServiceCollection services, Uri authority, string clientId, string clientSecret, string scope)
	{
		var config = new Helpers.Identity.Config(authority, clientId, clientSecret, scope);

		return services
			.AddSingleton(Options.Options.Create(config))
			.AddIdentityClient();
	}

	public static IServiceCollection AddIdentityClient(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.Configure<Helpers.Identity.Config>(configuration)
			.AddIdentityClient();
	}

	public static IServiceCollection AddIdentityClient(this IServiceCollection services, IOptions<Helpers.Identity.Config> options)
	{
		return services
			.AddSingleton(options)
			.AddIdentityClient();
	}

	/// <summary>
	/// needs a previously-injected <see cref="IOptions"/>&lt;<see cref=<Helpers.Identity.Config"/>&gt;
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/>.</param>
	/// <returns>The <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddIdentityClient(this IServiceCollection services)
	{
		return services
			.TryAddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
			.AddHttpClient<Helpers.Identity.Clients.IIdentityClient, Helpers.Identity.Clients.Concrete.IdentityClient>((provider, client) =>
			{
				var config = provider.GetRequiredService<IOptions<Helpers.Identity.Config>>().Value;
				client.BaseAddress = config.Authority;
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services;
	}
	#endregion AddIdentityClient

	#region AddAuthenticationAuthorization
	public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, IConfiguration configuration)
	{
		var config = Helpers.Identity.Config.Defaults;
		configuration.Bind(config);

		return services
			.AddAuthenticationAuthorization(config);
	}

	public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, IOptions<Helpers.Identity.Config> options)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;

		return services
			.AddAuthenticationAuthorization(config.Authority, config.Scope);
	}

	public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, Uri authority, string scope)
	{
		Guard.Argument(authority).NotNull().Wrap(uri => uri.OriginalString)
			.NotNull().NotEmpty().NotWhiteSpace();

		Guard.Argument(scope).NotNull().NotEmpty().NotWhiteSpace();

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{
				options.Authority = authority.OriginalString;

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
				policy.RequireClaim("Scope", scope);
			});
		});

		return services;
	}
	#endregion AddAuthenticationAuthorization

	private static IServiceCollection TryAddSingleton<TService>(this IServiceCollection services, TService instance)
		where TService : class
	{
		ServiceCollectionDescriptorExtensions.TryAddSingleton(services, instance);
		return services;
	}
}
