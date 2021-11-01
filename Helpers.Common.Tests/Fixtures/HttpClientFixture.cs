namespace Helpers.Common.Tests.Fixtures;

public sealed class HttpClientFixture : IDisposable
{
	private readonly HttpMessageHandler _handler;

	public HttpClientFixture()
	{
		_handler = new HttpClientHandler { AllowAutoRedirect = false, };
		HttpClient = new HttpClient(_handler);
	}

	public HttpClient HttpClient { get; }

	public void Dispose()
	{
		HttpClient?.Dispose();
		_handler?.Dispose();
	}
}
