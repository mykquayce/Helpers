using RabbitMQ.Client;

namespace Helpers.RabbitMQ.Concrete
{
	public class RabbitMQSettings : IRabbitMQSettings
	{
		public string HostName { get; set; } = "localhost";
		public string Password { get; set; } = ConnectionFactory.DefaultPass;
		public int Port { get; set; } = 5_672;
		public string UserName { get; set; } = ConnectionFactory.DefaultUser;
		public string VirtualHost { get; set; } = ConnectionFactory.DefaultVHost;
	}
}
