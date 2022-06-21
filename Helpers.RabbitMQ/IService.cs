using System.Text;
using System.Text.Json;

namespace Helpers.RabbitMQ;

public interface IService
{
	private static readonly Encoding _encoding = Encoding.UTF8;

	void Acknowledge(ulong tag);
	(byte[] body, ulong tag) Dequeue(string queue);
	(T body, ulong tag) Dequeue<T>(string queue)
	{
		var (body, tag) = Dequeue(queue);
		var json = _encoding.GetString(body);
		var t = JsonSerializer.Deserialize<T>(json);
		return (t!, tag);
	}
	void Enqueue(string queue, byte[] body);
	void Enqueue<T>(string queue, T value)
	{
		var json = JsonSerializer.Serialize(value);
		var body = _encoding.GetBytes(json);
		Enqueue(queue, body);
	}
	void PurgeQueue(string queue);
	void DeleteQueue(string queue);
}
