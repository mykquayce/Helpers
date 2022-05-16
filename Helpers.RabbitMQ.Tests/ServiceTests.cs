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
		catch (Exceptions.QueueNotFoundOrEmptyException)
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
		catch (Exceptions.QueueNotFoundOrEmptyException) { }

		// publish the message to queue 2
		_service.Enqueue(queues[1], result1.body);

		// acknowledge
		_service.Acknowledge(result1.tag);

		// consume a message from queue 1 (fails)
		try { _service.Dequeue(queues[0]); Assert.True(false); }
		catch (Exceptions.QueueNotFoundOrEmptyException) { }

		// consume a message from queue 2
		var (_, tag) = _service.Dequeue(queues[1]);

		// acknowledge
		_service.Acknowledge(tag);
	}

	[Theory]
	[InlineData("test-queue", "hello world")]
	public void DisposingTests(string queueName, string message)
	{
		{
			using IService service = new Concrete.Service(Concrete.Service.Config.Defaults);
			service.Enqueue(queueName, message);
		}

		{
			using IService service = new Concrete.Service(Concrete.Service.Config.Defaults);
			var (actual, tag) = service.Dequeue<string>(queueName);
			service.Acknowledge(tag);
			Assert.Equal(message, actual);
		}

		{
			using IService service = new Concrete.Service(Concrete.Service.Config.Defaults);
			service.PurgeQueue(queueName);
		}

		{
			using IService service = new Concrete.Service(Concrete.Service.Config.Defaults);
			service.DeleteQueue(queueName);
		}
	}

	[Theory]
	[InlineData("test-queue", "hello world")]
	public void PurgeTests(string queue, string message)
	{
		_service.Enqueue(queue, message);
		_service.PurgeQueue(queue);
		void testCode() => _service.Dequeue(queue);
		Assert.Throws<Exceptions.QueueNotFoundOrEmptyException>(testCode);
	}

	[Theory]
	[InlineData("test-queue", "hello world")]
	public void DeleteEmptyQueueTests(string queue, string message)
	{
		_service.Enqueue(queue, message);
		_service.DeleteQueue(queue);
		void testCode() => _service.Dequeue(queue);
		Assert.Throws<Exceptions.QueueNotFoundOrEmptyException>(testCode);
	}

	[Theory]
	[InlineData("rabbitmq", 5_671, "ssl-test", "hello world")]
	public void Test2(string hostName, ushort port, string queueName, string message)
	{
		var config = Concrete.Service.Config.Defaults with { Hostname = hostName, Port = port, SslEnabled = true, };
		using IService service = new Concrete.Service(config);
		service.Enqueue(queueName, message);
		var (actual, tag) = service.Dequeue<string>(queueName);

		Assert.NotNull(actual);
		Assert.Equal(message, actual);
		Assert.InRange(tag, 1ul, ulong.MaxValue);

		service.Acknowledge(tag);
		service.DeleteQueue(queueName);
	}

	[Theory]
	[InlineData("fpidhaiudthlriaulpdhapr")]
	public void DequeueNonExistentQueue(string queue)
	{
		void testCode() => _service.Dequeue(queue);
		Assert.Throws<Helpers.RabbitMQ.Exceptions.QueueNotFoundOrEmptyException>(testCode);
	}
}
