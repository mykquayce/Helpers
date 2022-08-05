using System.Xml.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddReddit(this IServiceCollection services)
	{
		return services
			.AddSingleton(new XmlSerializerFactory())
			.AddHttpClient<Helpers.Reddit.IClient, Helpers.Reddit.Concrete.Client>("reddit-client", client =>
			{
				client.BaseAddress = new Uri("https://old.reddit.com", UriKind.Absolute);
			})
			.ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.Services
			.AddTransient<Helpers.Reddit.IService, Helpers.Reddit.Concrete.Service>();
	}
}
