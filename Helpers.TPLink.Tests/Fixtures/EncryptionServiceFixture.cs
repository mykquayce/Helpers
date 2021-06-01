namespace Helpers.TPLink.Tests.Fixtures
{
	public class EncryptionServiceFixture
	{
		public IEncryptionService EncryptionService { get; } = new Concrete.EncryptionService();
	}
}
