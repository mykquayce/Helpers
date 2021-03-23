using System.Net;

namespace Helpers.Networking.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			BroadcastIPAddress = IPAddress.Parse(userSecretsFixture["GlobalCache:BroadcastIPAddress"]);
			HostName = userSecretsFixture["GlobalCache:HostName"];
			Port = ushort.Parse(userSecretsFixture["GlobalCache:Port"]);
			ReceivePort = ushort.Parse(userSecretsFixture["GlobalCache:ReceivePort"]);
		}

		public IPAddress BroadcastIPAddress { get; }
		public string HostName { get; }
		public ushort Port { get; }
		public ushort ReceivePort { get; }
	}
}
