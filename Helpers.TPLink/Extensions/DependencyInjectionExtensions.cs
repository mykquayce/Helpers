using System.Net.Sockets;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddTPLink(this IServiceCollection services)
	{
		return services
			.AddTransient<UdpClient>(_ => new UdpClient(AddressFamily.InterNetwork))
			.AddTransient<Helpers.TPLink.IClient, Helpers.TPLink.Concrete.Client>()
			.AddTransient<Helpers.TPLink.IDiscoveryClient, Helpers.TPLink.Concrete.DiscoveryClient>()
			.AddTransient<Helpers.TPLink.IService, Helpers.TPLink.Concrete.Service>();
	}
}
