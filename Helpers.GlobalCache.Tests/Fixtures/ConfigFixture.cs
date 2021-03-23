using System.Net;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public class ConfigFixture
	{
		public ConfigFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			BroadcastIPAddress = IPAddress.Parse(userSecretsFixture["GlobalCache:BroadcastIPAddress"]);
			HostName = userSecretsFixture["GlobalCache:HostName"];
			Port = ushort.Parse(userSecretsFixture["GlobalCache:Port"]);
			ReceivePort = ushort.Parse(userSecretsFixture["GlobalCache:ReceivePort"]);

			Config = new Services.Concrete.GlobalCacheService.Config(
				BroadcastIPAddress.ToString(),
				Port);
		}

		public IPAddress BroadcastIPAddress { get; }
		public string HostName { get; }
		public ushort Port { get; }
		public ushort ReceivePort { get; }

		public Helpers.GlobalCache.Services.Concrete.GlobalCacheService.Config Config { get; }
	}
}
