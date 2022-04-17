namespace Helpers.NetworkDiscoveryApi.Tests.Fixtures;

public sealed class SecureClientFixture : IdentityClientFixture, IDisposable
{
	private readonly HttpClient _httpClient;

	public SecureClientFixture()
	{
		var clientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(clientHandler) { BaseAddress = base.BaseAddress, };
		Client = new Concrete.SecureClient(_httpClient, IdentityClient);
	}

	public IClient Client { get; }

	public new void Dispose()
	{
		_httpClient.Dispose();
		base.Dispose();
	}
}
