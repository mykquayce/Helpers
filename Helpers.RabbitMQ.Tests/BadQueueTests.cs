using Xunit;

namespace Helpers.RabbitMQ.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class BadQueueTests : IClassFixture<Fixtures.ServiceProviderFixture>
{
	private readonly Helpers.RabbitMQ.IService _service;
	private readonly Helpers.RabbitMQ.Config _config;

	public BadQueueTests(Fixtures.ServiceProviderFixture fixture)
	{
		_service = fixture.Service;
		_config = fixture.Config;
	}

	[Theory]
	[InlineData("hello world")]
	public void EnqueueDequeueAcknowlegdeTests(string message)
	{
		var queue = _config.QueueNames.First();
		_service.Enqueue(queue, message);
		var (actual, tag) = _service.Dequeue<string>(queue);
		_service.Acknowledge(tag);
		Assert.InRange(tag, 1UL, ulong.MaxValue);
		Assert.Equal(message, actual);
		_service.PurgeQueue(queue);
	}

	[Theory]
	[InlineData("qwyfultjqwyuflpjqywfup")]
	public void Dequeue_EmptyQueueTests(string queue)
	{
		_service.CreateQueue(queue);

		void testCode() => _service.Dequeue(queue);

		try
		{
			Assert.Throws<Helpers.RabbitMQ.Exceptions.QueueEmptyException>(testCode);
		}
		finally
		{
			_service.DeleteQueue(queue);
		}

	}

	[Theory]
	[InlineData("arhstieanrhsitenharst")]
	public void Dequeue_MissingQueueTests(string queue)
	{
		Assert.False(_service.QueueExists(queue));

		try
		{
			void testCode() => _service.Dequeue(queue);

			Assert.Throws<Helpers.RabbitMQ.Exceptions.QueueNotFoundException>(testCode);
		}
		finally
		{
			_service.DeleteQueue(queue);
		}
	}

	[Theory]
	[InlineData("enrsteanroseinaorstie")]
	public void EnsureQueueExistsTests(string queue)
	{
		Assert.False(_service.QueueExists(queue));

		_service.EnsureQueueExists(queue);

		try
		{
			Assert.True(_service.QueueExists(queue));

			_service.DeleteQueue(queue);
		}
		finally
		{
			_service.DeleteQueue(queue);
		}
	}

	[Theory]
	[InlineData("oairesntoiaernstoiea", "hello world")]
	public void EnqueueToMissingQueueTests(string queue, string message)
	{
		Assert.False(_service.QueueExists(queue));

		try
		{
			_service.Enqueue(queue, message);

			Assert.True(_service.QueueExists(queue));

			var (actual, tag) = _service.Dequeue<string>(queue);

			Assert.Equal(message, actual);
			Assert.InRange(tag, 1UL, ulong.MaxValue);

			_service.Acknowledge(tag);
		}
		finally
		{
			_service.DeleteQueue(queue);
		}
	}
}
