using System.Text.Json;

namespace Helpers.NetworkDiscoveryApi.Concrete;

public class Client : IClient
{
	private readonly HttpClient _httpClient;

	public Client(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async IAsyncEnumerable<Models.DhcpResponseObject> GetLeasesAsync(CancellationToken? cancellationToken = default)
	{
		await using var stream = await _httpClient.GetStreamAsync("/api/router", cancellationToken ?? CancellationToken.None);
		var responses = JsonSerializer.DeserializeAsyncEnumerable<Models.DhcpResponseObject>(stream, cancellationToken: cancellationToken ?? CancellationToken.None);
		await using var enumerator = responses.GetAsyncEnumerator(cancellationToken ?? CancellationToken.None);
		while (await enumerator.MoveNextAsync(cancellationToken ?? CancellationToken.None))
		{
			yield return enumerator.Current!;
		}
	}
}
