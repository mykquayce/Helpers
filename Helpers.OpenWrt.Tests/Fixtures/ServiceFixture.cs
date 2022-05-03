namespace Helpers.OpenWrt.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly ClientFixture _clientFixture = new();

	public ServiceFixture()
	{
		Service = new Concrete.Service(_clientFixture.Client);
	}

	public IService Service { get; }

	public void Dispose() => _clientFixture.Dispose();
}
