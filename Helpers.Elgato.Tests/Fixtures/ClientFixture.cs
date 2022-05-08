namespace Helpers.Elgato.Tests.Fixtures;

public class ClientFixture : XUnitClassFixtures.HttpClientFixture
{
	public ClientFixture()
	{
		var config = Config.Defaults;
		var httpClient = base.HttpClient;

		Client = new Concrete.Client(config, httpClient);
	}

	public IClient Client { get; }
}
