namespace Helpers.Networking.Tests.Fixtures;

public class UdpClientFixture : UserSecretsFixture
{
	public UdpClientFixture()
	{
		Config = new Helpers.Networking.Clients.Concrete.UdpClient.Config(
			base.BroadcastIPAddress.ToString(),
			base.ReceivePort);

		UdpClient = new Clients.Concrete.UdpClient(Config);
	}

	public Helpers.Networking.Clients.Concrete.UdpClient.Config Config { get; }
	public Helpers.Networking.Clients.Concrete.UdpClient UdpClient { get; }
}
