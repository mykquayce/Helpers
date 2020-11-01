namespace Helpers.TPLink.Tests.Fixtures
{
	public class EncryptionServiceFixture
	{
		public EncryptionServiceFixture()
		{
			EncryptionService = new Concrete.EncryptionService(0xAB);
		}

		public IEncryptionService EncryptionService { get; }
	}
}
