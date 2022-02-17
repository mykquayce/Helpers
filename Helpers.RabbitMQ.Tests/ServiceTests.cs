using Xunit;

namespace Helpers.RabbitMQ.Tests;

public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _service;

	public ServiceTests(Fixtures.ServiceFixture fixture)
	{
		_service = fixture.Service;
	}

	[Theory]
	[InlineData("test", "message")]
	public void PublishConsumeAcknowledge(string queue, string message)
	{
		var before = new Class { String = message, };

		_service.Enqueue(queue, before);

		var (actual, deliveryTag) = _service.Dequeue<Class>(queue);

		Assert.NotNull(actual);
		Assert.Equal(message, actual.String);
		Assert.NotEqual(default, deliveryTag);

		_service.Acknowledge(deliveryTag);
	}

	[Theory]
	[InlineData("test", "https://old.reddit.com/r/random/.rss")]
	public void ConsumePublishConsumeAckhowledge(string queue, string message)
	{
		var bytes = System.Text.Encoding.UTF8.GetBytes(message);

		try
		{
			_service.Dequeue(queue);
			Assert.True(false);
		}
		catch (Exceptions.QueueEmptyException)
		{ }
		catch (Exceptions.QueueNotFoundException)
		{ }

		_service.Enqueue(queue, bytes);

		var (actualBytes, deliveryTag) = _service.Dequeue(queue);

		Assert.NotNull(actualBytes);

		var actual = System.Text.Encoding.UTF8.GetString(actualBytes);

		Assert.Equal(message, actual);
		Assert.NotEqual(default, deliveryTag);

		_service.Acknowledge(deliveryTag);
	}

	private class Class { public string? String { get; set; } }
	[Theory]
	[InlineData("first-queue", "second-queue")]
	public void MultipleQueueTests(params string[] queues)
	{
		byte[] message = System.Text.Encoding.UTF8.GetBytes("hello world");

		// publish a message to queue 1
		_service.Enqueue(queues[0], message);

		// consume a message from queue 1
		var result1 = _service.Dequeue(queues[0]);

		// consume a message from queue 2 (fails)
		try { _service.Dequeue(queues[1]); Assert.True(false); }
		catch (Exceptions.QueueEmptyException) { }

		// publish the message to queue 2
		_service.Enqueue(queues[1], result1.body);

		// acknowledge
		_service.Acknowledge(result1.tag);

		// consume a message from queue 1 (fails)
		try { _service.Dequeue(queues[0]); Assert.True(false); }
		catch (Exceptions.QueueEmptyException) { }

		// consume a message from queue 2
		var (_, tag) = _service.Dequeue(queues[1]);

		// acknowledge
		_service.Acknowledge(tag);
	}
}
