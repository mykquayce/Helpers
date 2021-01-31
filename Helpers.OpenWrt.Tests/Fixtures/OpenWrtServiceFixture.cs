namespace Helpers.OpenWrt.Tests.Fixtures
{
	public class OpenWrtServiceFixture
	{
		public OpenWrtServiceFixture()
		{
			var clientFixture = new OpenWrtClientFixture();

			OpenWrtService = new Services.Concrete.OpenWrtService(clientFixture.OpenWrtClient);
		}

		public Services.IOpenWrtService OpenWrtService { get; }
	}
}
