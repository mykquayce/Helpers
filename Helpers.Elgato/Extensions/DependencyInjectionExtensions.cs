using Helpers.Elgato;
using Helpers.Elgato.Concrete;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IHttpClientBuilder AddElgato(this IServiceCollection services, Uri baseAddress)
		=> AddElgato(services, client => client.BaseAddress = baseAddress);

	public static IHttpClientBuilder AddElgato(this IServiceCollection services, Action<HttpClient> configureClient)
	{
		return services
			.AddTransient<IWhiteLightService, WhiteLightService>()
			.AddHttpClient<IWhiteLightClient, WhiteLightClient>(configureClient);
	}

	public static IHttpClientBuilder AddElgato(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient)
	{
		return services
			.AddTransient<IWhiteLightService, WhiteLightService>()
			.AddHttpClient<IWhiteLightClient, WhiteLightClient>(configureClient);
	}
}
