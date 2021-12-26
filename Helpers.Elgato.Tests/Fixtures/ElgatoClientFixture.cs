namespace Helpers.Elgato.Tests.Fixtures;

public class ElgatoClientFixture
{
	public IElgatoClient Client { get; } = new Concrete.ElgatoClient(Concrete.ElgatoClient.Config.Defaults);
}
