using Microsoft.Extensions.DependencyInjection;

namespace Helpers.GlobalCache.Tests;

[Collection(nameof(NonParallelCollection))]
public class DependencyInjectionTests : IClassFixture<Fixtures.ServiceProviderFixture>
{
	private readonly IServiceProvider _serviceProvider;

	public DependencyInjectionTests(Fixtures.ServiceProviderFixture fixture)
	{
		_serviceProvider = fixture.ServiceProvider;
	}

	[Theory]
	[InlineData(2,
		"sendir,1:1,1,40192,2,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
		"completeir")]
	public async Task SendTests(int count, string message, string expected)
	{
		while (count-- > 0)
		{
			var sut = _serviceProvider.GetRequiredService<IClient>();
			var response = await sut.SendAsync(message);
			Assert.NotNull(response);
			Assert.StartsWith(expected, response, StringComparison.OrdinalIgnoreCase);
			await Task.Delay(millisecondsDelay: 1_000);
		}
	}
}
