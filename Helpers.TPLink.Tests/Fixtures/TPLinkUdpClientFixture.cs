namespace Helpers.TPLink.Tests.Fixtures
{
	public class TPLinkUdpClientFixture
	{
		public TPLinkUdpClientFixture()
		{
			var config = new Concrete.TPLinkUdpClient.Config
			{
				MillisecondsTimeout = 5_000,
				Port = 9_999,
			};

			var encryptionService = new Concrete.EncryptionService(initialKey: 0xAB);

			TPLinkUdpClient = new Concrete.TPLinkUdpClient(config, encryptionService);
		}

		public ITPLinkUdpClient TPLinkUdpClient { get; }
	}
}
