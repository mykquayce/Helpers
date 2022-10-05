namespace Helpers.Elgato.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly ClientFixture _elgatoClientFixture = new();

	public ServiceFixture()
	{
		Service = new Concrete.Service(_elgatoClientFixture.Client);
	}

	public IService Service { get; }

	public void Dispose() => _elgatoClientFixture.Dispose();
}
