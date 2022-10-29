using Microsoft.Extensions.DependencyInjection;

namespace Helpers.GlobalCache.Tests.Fixtures;

public class ClientFixture : ServiceProviderFixture
{
	public IClient Client => ServiceProvider.GetRequiredService<IClient>();
}
