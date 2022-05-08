namespace Helpers.Elgato.Tests.Fixtures;

public class ServiceFixture : IDisposable
{
	private readonly ClientFixture _elgatoClientFixture = new();

	public ServiceFixture()
	{
		Service = new Concrete.Service(_elgatoClientFixture.Client);
	}

	public IService Service { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "<Pending>")]
	public void Dispose() => _elgatoClientFixture.Dispose();
}
