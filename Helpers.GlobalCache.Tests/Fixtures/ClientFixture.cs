namespace Helpers.GlobalCache.Tests.Fixtures
{
	public class ClientFixture
	{
		public IClient Client { get; } = new Concrete.Client(Config.Defaults);
	}
}
