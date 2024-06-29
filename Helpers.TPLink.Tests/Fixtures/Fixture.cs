using Microsoft.Extensions.DependencyInjection;

namespace Helpers.TPLink.Tests.Fixtures;

public sealed class Fixture : IAsyncDisposable, IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		_serviceProvider = new ServiceCollection()
			.AddTPLink()
			.BuildServiceProvider();

		Service = _serviceProvider.GetRequiredService<IService>();
	}

	public void Dispose() => ((ServiceProvider)_serviceProvider).Dispose();
	public ValueTask DisposeAsync() => ((ServiceProvider)_serviceProvider).DisposeAsync();

	public IService Service { get; }
}
