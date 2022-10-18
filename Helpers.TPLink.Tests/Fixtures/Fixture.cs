using Microsoft.Extensions.Caching.Memory;

namespace Helpers.TPLink.Tests.Fixtures;

public class Fixture
{
	private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
	public Fixture()
	{
		var config = Config.Defaults;
		Client = new Concrete.TPLinkClient(config);
		Service = new Concrete.TPLinkService(Client, _cache);
	}

	public ITPLinkClient Client { get; }
	public ITPLinkService Service { get; }
}
