using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Helpers.RabbitMQ;

public interface IService
{
	private static readonly Encoding _encoding = Encoding.UTF8;

	ValueTask AcknowledgeAsync(ulong tag, CancellationToken cancellationToken = default);
	Task<QueueDeclareOk> CreateQueueAsync(string queue, CancellationToken cancellationToken = default);
	ValueTask<(byte[] body, ulong tag)> DequeueAsync(string queue, bool autoAcknowledge = false, CancellationToken cancellationToken = default);
	async ValueTask<(T body, ulong tag)> DequeueAsync<T>(string queue, bool autoAcknowledge = false, CancellationToken cancellationToken = default)
	{
		var (body, tag) = await DequeueAsync(queue, autoAcknowledge, cancellationToken);
		var json = _encoding.GetString(body);
		var t = JsonSerializer.Deserialize<T>(json);
		return (t!, tag);
	}
	ValueTask EnqueueAsync(string queue, byte[] body, CancellationToken cancellationToken = default);
	ValueTask EnqueueAsync<T>(string queue, T value, CancellationToken cancellationToken = default)
	{
		var json = JsonSerializer.Serialize(value);
		var body = _encoding.GetBytes(json);
		return EnqueueAsync(queue, body, cancellationToken);
	}
	Task<uint> PurgeQueueAsync(string queue, CancellationToken cancellationToken = default);
	Task<uint> DeleteQueueAsync(string queue, CancellationToken cancellationToken = default);
}
