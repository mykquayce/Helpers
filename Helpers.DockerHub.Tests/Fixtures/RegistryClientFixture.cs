namespace Helpers.DockerHub.Tests.Fixtures;

public sealed class RegistryClientFixture : IDisposable
{
	private readonly HttpMessageHandler _httpMessageHandler;
	private readonly HttpClient _httpClient;
	private readonly AuthorizationClientFixture _authorizationClientFixture;

	public RegistryClientFixture()
	{
		_authorizationClientFixture = new AuthorizationClientFixture();
		_httpMessageHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(_httpMessageHandler) { BaseAddress = Config.DefaultRegistryApiBaseAddress, };
		RegistryClient = new Concrete.RegistryClient(_httpClient, _authorizationClientFixture.AuthorizationClient);
	}

	public IRegistryClient RegistryClient { get; }

	public void Dispose()
	{
		_authorizationClientFixture.Dispose();
		_httpClient.Dispose();
		_httpMessageHandler.Dispose();
	}
}
