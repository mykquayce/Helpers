namespace Helpers.RabbitMQ;

public interface IClient
{
	IAsyncEnumerable<Models.QueueObject> GetQueuesAsync(CancellationToken? cancellationToken = null);
}
