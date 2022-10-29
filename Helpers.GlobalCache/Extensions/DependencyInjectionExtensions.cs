using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Sockets;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddGlobalCache(
		this IServiceCollection services,
		Helpers.GlobalCache.Config config,
		Helpers.GlobalCache.Models.MessagesDictionary messagesDictionary)
	{
		return services
			.AddSingleton<IOptions<Helpers.GlobalCache.Config>>(Options.Options.Create(config))
			.AddSingleton<IOptions<Helpers.GlobalCache.Models.MessagesDictionary>>(Options.Options.Create(messagesDictionary))
			.AddGlobalCache();
	}

	public static IServiceCollection AddGlobalCache(
		this IServiceCollection services,
		IConfiguration configuration,
		IConfiguration messagesDictionaryConfiguration)
	{
		return services
			.Configure<Helpers.GlobalCache.Config>(configuration)
			.Configure<Helpers.GlobalCache.Models.MessagesDictionary>(messagesDictionaryConfiguration)
			.AddGlobalCache();
	}

	public static IServiceCollection AddGlobalCache(
		this IServiceCollection services)
	{
		return services
			.AddTransient(_ => new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			.AddTransient<Helpers.GlobalCache.IClient, Helpers.GlobalCache.Concrete.Client>()
			.AddTransient<Helpers.GlobalCache.IService, Helpers.GlobalCache.Concrete.Service>();
	}
}
