namespace Helpers.TPLink.Tests.Fixtures
{
	public class TPLinkClientFixture
	{
		public TPLinkClientFixture()
		{
			var deviceCache = new Concrete.DeviceCache();
			var encryptionService = new EncryptionServiceFixture().EncryptionService;

			TPLinkClient = new Concrete.TPLinkClient(deviceCache, encryptionService);
		}

		public ITPLinkClient TPLinkClient { get; }
	}
}
