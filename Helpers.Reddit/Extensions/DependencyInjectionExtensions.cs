using Helpers.Reddit;
using Helpers.Reddit.Concrete;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IHttpClientBuilder AddReddit(this IServiceCollection services, Uri baseAddress)
	{
		return services
			.AddTransient<IService, Service>()
			.AddHttpClient<IClient, Client>(name: "reddit-client", c => c.BaseAddress = baseAddress);
	}
}
