namespace Helpers.Networking.Tests.Fixtures
{
	public class PingClientFixture
	{
		public PingClientFixture()
		{
			var config = Clients.Concrete.PingClient.Config.Defaults;
			PingClient = new Clients.Concrete.PingClient(config);
		}

		public Clients.IPingClient PingClient { get; }
	}
}
