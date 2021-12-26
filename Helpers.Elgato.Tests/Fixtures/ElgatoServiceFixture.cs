namespace Helpers.Elgato.Tests.Fixtures;

public class ElgatoServiceFixture
{
	public ElgatoServiceFixture()
	{
		var clientFixture = new ElgatoClientFixture();
		Service = new Concrete.ElgatoService(clientFixture.Client);
	}

	public IElgatoService Service { get; }
}
