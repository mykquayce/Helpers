using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddElgato(this IServiceCollection services, string scheme, int port)
	{
		var config = new Helpers.Elgato.Config(scheme, port);
		return services
			.AddElgato(config);
	}

	public static IServiceCollection AddElgato(this IServiceCollection services, IOptions<Helpers.Elgato.Config> config)
	{
		return services
			.AddSingleton(config)
			.AddElgato();
	}

	public static IServiceCollection AddElgato(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.Configure<Helpers.Elgato.Config>(configuration)
			.AddElgato();
	}

	public static IServiceCollection AddElgato(this IServiceCollection services)
	{
		return services
			.AddHttpClient<Helpers.Elgato.IClient, Helpers.Elgato.Concrete.Client>(name: "elgato-client")
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			.AddTransient<Helpers.Elgato.IService, Helpers.Elgato.Concrete.Service>();
	}
}
