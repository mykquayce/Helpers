namespace Helpers.NetworkDiscovery.Tests;

public class TestClient(HttpClient httpClient)
{
	public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
		=> httpClient.GetAsync(requestUri, cancellationToken);
}
