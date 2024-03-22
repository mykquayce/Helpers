namespace Helpers.DelegatingHandlers.Tests;

public class TestClient(HttpClient httpClient)
{
	public Task<byte[]> GetByteArrayAsync(int count, CancellationToken cancellationToken = default)
		=> httpClient.GetByteArrayAsync("bytes/" + count, cancellationToken);
}
