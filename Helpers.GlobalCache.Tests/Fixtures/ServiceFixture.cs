using Microsoft.Extensions.DependencyInjection;

namespace Helpers.GlobalCache.Tests.Fixtures;

public class ServiceFixture : ClientFixture
{
	public IService Service => ServiceProvider.GetRequiredService<IService>();
}
