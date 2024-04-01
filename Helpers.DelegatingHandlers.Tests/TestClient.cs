namespace Helpers.DelegatingHandlers.Tests;

public class TestClient(HttpClient httpClient)
{
	public Task<byte[]> GetByteArrayAsync(int count, CancellationToken cancellationToken = default)
		=> httpClient.GetByteArrayAsync("bytes/" + count, cancellationToken);

	public async IAsyncEnumerable<KeyValuePair<string, string[]>> GetHeadersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var response = await httpClient.GetAsync(requestUri: "headers", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

		var o = await response.Content.ReadFromJsonAsync<HeadersResponseObject>(cancellationToken);

		foreach (var (key, values) in o.headers)
		{
			yield return new(key, [.. values]);
		}
	}

	public async Task<string> GetStatusCodeAsync(short code, CancellationToken cancellationToken = default)
	{
		var resopnse = await httpClient.GetAsync("status/" + code, cancellationToken);
		var content = await resopnse.Content.ReadAsStringAsync(cancellationToken);
		return content;
	}
}
