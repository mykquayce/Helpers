using Microsoft.Extensions.Options;

namespace Helpers.Networking.Tests.Fixtures
{
	public class PingClientFixture
	{
		public PingClientFixture()
		{
			var config = Clients.Concrete.PingClient.Config.Defaults;
			var options = Options.Create(config);
			PingClient = new Clients.Concrete.PingClient(options);
		}

		public Clients.IPingClient PingClient { get; }
	}
}
