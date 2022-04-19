namespace Helpers.PhilipsHue.Tests.Fixtures;

public sealed class ClientFixture : IDisposable
{
	private const string _section = "PhilipsHue";

	private readonly HttpClient _httpClient;

	public ClientFixture()
	{
		var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();
		var config = userSecretsFixture.GetSection<Config>(_section);
		var baseAddress = new Uri("http://" + config.Hostname);
		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(handler) { BaseAddress = baseAddress, };
		Client = new Concrete.Client(_httpClient, config);
	}

	public IClient Client { get; }

	public void Dispose() => _httpClient.Dispose();
}
