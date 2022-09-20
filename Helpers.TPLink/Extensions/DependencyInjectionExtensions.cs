using Helpers.TPLink;
using Helpers.TPLink.Concrete;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddTPLink(this IServiceCollection services, ushort port)
	{
		var config = new Config(port);
		return services.AddTPLink(config);
	}

	public static IServiceCollection AddTPLink(this IServiceCollection services, Config config)
	{
		return services
			.AddSingleton<IOptions<Config>>(config)
			.AddSingleton<IDeviceCache>(new DeviceCache())
			.AddTransient<ITPLinkClient, TPLinkClient>()
			.AddTransient<ITPLinkService, TPLinkService>();
	}
}
