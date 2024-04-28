using Helpers.Nanoleaf;
using Helpers.Nanoleaf.Concrete;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
	public static IServiceCollection AddNanoleaf(this IServiceCollection services, IConfiguration configuration)
	{
		var config = new Config();
		configuration.Bind(config);
		return services.AddNanoleaf(config);
	}

	public static IServiceCollection AddNanoleaf(this IServiceCollection services, Uri baseAddress, string token)
	{
		var config = new Config { BaseAddress = baseAddress, Token = token};
		return services.AddNanoleaf(config);
	}

	public static IServiceCollection AddNanoleaf(this IServiceCollection services, Action<Client.IConfig> configBuilder)
	{
		Client.IConfig config = new Config();
		configBuilder(config);
		return services.AddNanoleaf(config);
	}

	public static IServiceCollection AddNanoleaf(this IServiceCollection services, Client.IConfig config)
	{
		var options = Options.Options.Create(config);

		return services
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddSingleton(options)
			.AddHttpClient<IClient, Client>(name: "nanoleaf-client", client =>
			{
				client.BaseAddress = config.BaseAddress;
			})
			.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
			.Services;
	}

	private class Config : Client.IConfig
	{
		public string Token { get; set; }
		public Uri BaseAddress { get; set; }
	}
}
