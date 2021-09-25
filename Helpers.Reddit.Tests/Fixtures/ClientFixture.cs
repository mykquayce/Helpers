using System.Xml.Serialization;

namespace Helpers.Reddit.Tests.Fixtures;

public class ClientFixture : IDisposable
{
	private readonly HttpClientHandler _httpClientHandler;
	private readonly HttpClient _httpClient;

	public ClientFixture()
	{
		var baseAddress = new Uri("https://old.reddit.com", UriKind.Absolute);
		_httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(_httpClientHandler) { BaseAddress = baseAddress, };

		var xmlSerializerFactory = new XmlSerializerFactory();

		Client = new Helpers.Reddit.Concrete.Client(_httpClient, xmlSerializerFactory);
	}

	public Helpers.Reddit.IClient Client { get; }

	#region disposing
	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_httpClient.Dispose();
				_httpClientHandler.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion disposing
}
