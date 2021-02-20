namespace Helpers.GlobalCache.Tests.Fixtures
{
	public class ConfigFixture
	{
		public ConfigFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			var broadcastIPAddress = userSecretsFixture["GlobalCache:BroadcastIPAddress"];
			var physicalAddress = userSecretsFixture["GlobalCache:PhysicalAddress"];
			var port = ushort.Parse(userSecretsFixture["GlobalCache:Port"]);
			var receivePort = ushort.Parse(userSecretsFixture["GlobalCache:ReceivePort"]);

			Config = new Services.Concrete.GlobalCacheService.Config(
				broadcastIPAddress,
				physicalAddress,
				port,
				receivePort);
		}

		public Helpers.GlobalCache.Services.Concrete.GlobalCacheService.Config Config { get; }
	}
}
