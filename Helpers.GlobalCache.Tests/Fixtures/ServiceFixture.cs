namespace Helpers.GlobalCache.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	public IService Service { get; } = new Concrete.Service(Config.Defaults);

	public void Dispose() => Service.Dispose();
}
