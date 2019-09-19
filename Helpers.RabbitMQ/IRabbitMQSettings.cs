namespace Helpers.RabbitMQ
{
	public interface IRabbitMQSettings
	{
		string HostName { get; }
		string Password { get; }
		int Port { get; }
		string UserName { get; }
		string VirtualHost { get; }
	}
}
