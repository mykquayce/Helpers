using Helpers.Nanoleaf;
using Helpers.Nanoleaf.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
	public static IServiceCollection AddNanoleaf(this IServiceCollection services, Uri baseAddress, string token)
	{
		var config = new Config(baseAddress, token);
		return services.AddNanoleaf(config);
	}

	public static IServiceCollection AddNanoleaf(this IServiceCollection services, Config config)
	{
		return services
			.AddSingleton(Options.Options.Create(config))
			.AddNanoleaf();
	}

	public static IServiceCollection AddNanoleaf(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.Configure<Config>(configuration)
			.AddNanoleaf();
	}

	public static IServiceCollection AddNanoleaf(this IServiceCollection services)
	{
		return services
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddHttpClient<IClient, Client>("nanoleaf-client", (provider, client) =>
			{
				var config = provider.GetRequiredService<IOptions<Config>>().Value;
				client.BaseAddress = config.BaseAddress;
			})
			.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
			.Services;
	}
}
