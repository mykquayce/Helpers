namespace Helpers.XUnitClassFixtures;

public class HttpClientFixture : IDisposable
{
	private bool disposedValue;

	public HttpClientFixture()
	{
		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		HttpClient = new HttpClient(handler);
	}

	public HttpClient HttpClient { get; }

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				HttpClient.Dispose();
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
