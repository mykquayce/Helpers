using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Tests.Fixtures;

public class UserSecretsFixture
{
	public UserSecretsFixture()
	{
		var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		T f<T>(string key, Func<string, T> parser)
		{
			var value = userSecretsFixture!["Networking:GlobalCache:" + key]
				?? throw new KeyNotFoundException(key);
			return parser(value);
		}

		BroadcastIPAddress = f("BroadcastIPAddress", IPAddress.Parse);
		BroadcastPort = f("BroadcastPort", ushort.Parse);
		HostName = f("HostName", s => s);
		PhysicalAddress = f("PhysicalAddress", PhysicalAddress.Parse);
		ReceivePort = f("ReceivePort", ushort.Parse);
	}

	public IPAddress BroadcastIPAddress { get; }
	public ushort BroadcastPort { get; }
	public string HostName { get; }
	public PhysicalAddress PhysicalAddress { get; }
	public ushort ReceivePort { get; }
}
