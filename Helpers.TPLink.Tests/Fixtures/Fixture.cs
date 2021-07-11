namespace Helpers.TPLink.Tests.Fixtures
{
	public class Fixture
	{
		public Fixture()
		{
			var config = Config.Defaults;
			Client = new Concrete.TPLinkClient(config);
			Service = new Concrete.TPLinkService(Client);
		}

		public ITPLinkClient Client { get; }
		public ITPLinkService Service { get; }
	}
}
