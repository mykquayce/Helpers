using System.Net.Sockets;

namespace Helpers.Networking.Tests.Fixtures;

public class SocketClientFixture : UserSecretsFixture
{
	public SocketClientFixture()
	{
		Config = new Helpers.Networking.Clients.Concrete.SocketClient.Config(
			1_024,
			AddressFamily.InterNetwork,
			ProtocolType.Tcp,
			SocketType.Stream);

		SocketClient = new Clients.Concrete.SocketClient(Config);
	}

	public Helpers.Networking.Clients.Concrete.SocketClient.Config Config { get; }
	public Helpers.Networking.Clients.Concrete.SocketClient SocketClient { get; }
}
