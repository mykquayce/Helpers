using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Nanoleaf.Tests;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddNanoleaf(Constants.BaseAddress, Constants.Token)
			.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
	}

	public IClient Client { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
