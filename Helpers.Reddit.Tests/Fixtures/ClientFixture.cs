using System.Xml.Serialization;

namespace Helpers.Reddit.Tests.Fixtures;

public sealed class ClientFixture : IDisposable
{
	private readonly HttpClient _httpClient;

	public ClientFixture()
	{
		var baseAddress = new Uri("https://old.reddit.com", UriKind.Absolute);
		var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(httpClientHandler) { BaseAddress = baseAddress, };

		var xmlSerializerFactory = new XmlSerializerFactory();

		Client = new Concrete.Client(_httpClient, xmlSerializerFactory);
	}

	public IClient Client { get; }

	public void Dispose()
	{
		_httpClient.Dispose();
	}
}
