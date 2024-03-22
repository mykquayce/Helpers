using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace Helpers.DelegatingHandlers.Tests;

public class MockingHandlerTests
{
	[Theory]
	[InlineData(10)]
	public async Task Test1(int count)
	{
		var guids = new List<Guid>(capacity: count);

		var provider = new ServiceCollection()
			.AddSingleton(RandomNumberGenerator.Create())
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddTransient<MockingHandler>()
			.AddHttpClient<TestClient>(c => c.BaseAddress = new Uri("http://localhost/"))
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<MockingHandler>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<TestClient>();

		while (count-- > 0)
		{
			var bytes = await sut.GetByteArrayAsync(count: 16);
			var guid = new Guid(bytes);
			guids.Add(guid);
		}

		Assert.DoesNotContain(default, guids);
		Assert.Equal(guids.Count, guids.Distinct().Count());
	}
}
