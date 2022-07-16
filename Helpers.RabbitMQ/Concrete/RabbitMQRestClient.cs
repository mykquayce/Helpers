using System.Text.Json;

namespace Helpers.RabbitMQ.Concrete;

public class RabbitMQRestClient : IClient
{
	private readonly HttpClient _httpClient;

	public RabbitMQRestClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async IAsyncEnumerable<Models.QueueObject> GetQueuesAsync(CancellationToken? cancellationToken = null)
	{
		using var response = await _httpClient.GetAsync("api/queues", cancellationToken ?? CancellationToken.None);
		await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken ?? CancellationToken.None);
		var queues = JsonSerializer.DeserializeAsyncEnumerable<Models.QueueObject>(stream, cancellationToken: cancellationToken ?? CancellationToken.None);

		await foreach (var queue in queues)
		{
			if (queue is null) continue;
			yield return queue;
		}
	}
}
