using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddIdentityClient(this IServiceCollection services, IConfiguration configuration)
	{
		Guard.Argument(configuration).NotNull();
		var authority = configuration[nameof(Helpers.Identity.Config.Authority)];
		Guard.Argument(authority!).NotNull().NotEmpty().NotWhiteSpace();

		return services
			.Configure<Helpers.Identity.Config>(configuration)
			.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
			.AddHttpClient<Helpers.Identity.Clients.IIdentityClient, Helpers.Identity.Clients.Concrete.IdentityClient>(client =>
			{
				client.BaseAddress = new Uri(authority!, UriKind.Absolute);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services;
	}
}
