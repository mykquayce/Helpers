using Microsoft.Extensions.DependencyInjection;

namespace Helpers.TPLink.Tests.Fixtures;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddTPLink()
			.BuildServiceProvider();

		Client = get<IClient>();
		DiscoveryClient = get<IDiscoveryClient>();
		Service = get<IService>();

		T get<T>() where T : notnull
			=> _serviceProvider.GetRequiredService<T>();
	}

	public void Dispose() => ((ServiceProvider)_serviceProvider).Dispose();

	public IClient Client { get; }
	public IService Service { get; }
	public IDiscoveryClient DiscoveryClient { get; }
}
