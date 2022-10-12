using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddSSH(this IServiceCollection services, string host, int port, string username, string? password = null, string? pathToPrivateKey = null)
	{
		var config = new Helpers.SSH.Config(host, port, username, password, pathToPrivateKey);
		return services
			.AddSSH(config);
	}

	public static IServiceCollection AddSSH(this IServiceCollection services, Helpers.SSH.Config config)
	{
		return services
			.AddSingleton(Options.Options.Create(config))
			.AddSSH();
	}

	public static IServiceCollection AddSSH(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.Configure<Helpers.SSH.Config>(configuration)
			.AddSSH();
	}

	public static IServiceCollection AddSSH(this IServiceCollection services)
	{
		return services
			.AddScoped<Helpers.SSH.IClient, Helpers.SSH.Concrete.Client>()
			.AddScoped<Helpers.SSH.IService, Helpers.SSH.Concrete.Service>();
	}
}
