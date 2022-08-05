using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Reddit.Tests;

public class DependencyInjectionTests
{
	[Fact]
	public async Task Test1()
	{
		using var serviceProvider = new ServiceCollection()
			.AddReddit()
			.BuildServiceProvider();

		IService? sut = serviceProvider.GetService<Helpers.Reddit.IService>();

		Assert.NotNull(sut);

		var subredditName = await sut.GetRandomSubredditNameAsync();

		Assert.NotNull(subredditName);
		Assert.NotEmpty(subredditName);
	}
}
