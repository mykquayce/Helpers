namespace Helpers.NetworkDiscoveryApi.Tests.Fixtures;

public sealed class ClientFixture : ConfigurationFixture, IDisposable
{
	private readonly HttpClient _httpClient;

	public ClientFixture()
	{
		var clientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(clientHandler) { BaseAddress = base.BaseAddress, };
		Client = new Concrete.Client(_httpClient);
	}

	public IClient Client { get; }

	public void Dispose() => _httpClient.Dispose();
}
