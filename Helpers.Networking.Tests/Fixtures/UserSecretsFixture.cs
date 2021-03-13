using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			BroadcastIPAddress = IPAddress.Parse(userSecretsFixture["GlobalCache:BroadcastIPAddress"]);
			PhysicalAddress = PhysicalAddress.Parse(userSecretsFixture["GlobalCache:PhysicalAddress"]);
			Port = ushort.Parse(userSecretsFixture["GlobalCache:Port"]);
			ReceivePort = ushort.Parse(userSecretsFixture["GlobalCache:ReceivePort"]);
		}

		public IPAddress BroadcastIPAddress { get; }
		public PhysicalAddress PhysicalAddress { get; }
		public ushort Port { get; }
		public ushort ReceivePort { get; }
	}
}
