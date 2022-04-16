namespace Helpers.Elgato.Tests.Fixtures;

public class ElgatoClientFixture : Helpers.XUnitClassFixtures.UserSecretsFixture
{
	private readonly HttpClient _httpClient;

	public ElgatoClientFixture()
	{
		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri(base.Configuration["Elgato:IPAddress"]),
		};

		Client = new Helpers.Elgato.Concrete.ElgatoClient(_httpClient);
	}

	public IElgatoClient Client { get; }
}
