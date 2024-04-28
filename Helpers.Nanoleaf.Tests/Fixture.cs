using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Nanoleaf.Tests;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddNanoleaf(Constants.BaseAddress, Constants.Token)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.Services
			.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
	}

	public IClient Client { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
