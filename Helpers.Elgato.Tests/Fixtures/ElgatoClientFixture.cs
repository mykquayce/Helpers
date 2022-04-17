using System.Net;

namespace Helpers.Elgato.Tests.Fixtures;

public class ElgatoClientFixture
{
	private readonly HttpClient _httpClient;

	public ElgatoClientFixture()
	{
		Uri baseAddress;
		{
			var @base = new XUnitClassFixtures.UserSecretsFixture();
			var scheme = Config.DefaultScheme;
			var ip = IPAddress.Parse(@base["Elgato:IPAddress"]);
			var port = Config.DefaultPort;
			baseAddress = new Uri($"{scheme}://{ip}:{port}");
		}

		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(handler)
		{
			BaseAddress = baseAddress,
		};

		Client = new Helpers.Elgato.Concrete.ElgatoClient(_httpClient);
	}

	public IElgatoClient Client { get; }
}
