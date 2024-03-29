﻿using System.Text;
using System.Text.Json;

namespace Helpers.RabbitMQ;

public interface IService
{
	private static readonly Encoding _encoding = Encoding.UTF8;

	void Acknowledge(ulong tag);
	void CreateQueue(string queue);
	(byte[] body, ulong tag) Dequeue(string queue, bool autoAcknowledge = false);
	(T body, ulong tag) Dequeue<T>(string queue, bool autoAcknowledge = false)
	{
		var (body, tag) = Dequeue(queue, autoAcknowledge);
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
	void Enqueue<T>(string queue, T value, params T[] values)
	{
		Enqueue(queue, value);
		Enqueue(queue, values);
	}
	void Enqueue<T>(string queue, IEnumerable<T> values)
	{
		foreach (var value in values)
		{
			Enqueue(queue, value);
		}
	}
	void PurgeQueue(string queue);
	void DeleteQueue(string queue);
}
