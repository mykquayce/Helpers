using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Helpers.DelegatingHandlers.Tests;

public class RateLimitHandlerTests
{
	[Theory, InlineData("00:00:01", 1, 1)]
	public async Task Test1(string replenishmentPeriod, int tokenLimit, int tokensPerPeriod)
	{
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddRateLimitHandler(replenishmentPeriod: TimeSpan.Parse(replenishmentPeriod), tokenLimit: tokenLimit, tokensPerPeriod: tokensPerPeriod)
			.AddTransient<MockingHandler>()
			.AddHttpClient<Client>()
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<RateLimitHandler>()
				.AddHttpMessageHandler<MockingHandler>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<Client>();

		var stopwatch = Stopwatch.StartNew();
		var count = 10;

		while (count-- > 0)
		{
			await sut.GetAsync();
		}

		stopwatch.Stop();

		Assert.InRange(stopwatch.Elapsed.TotalSeconds, 8, 12);
	}

	private class Client(HttpClient httpClient)
	{
		public Task<HttpResponseMessage> GetAsync() => httpClient.GetAsync("http://localhost/");
	}

	private class MockingHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = new HttpResponseMessage();
			return Task.FromResult(response);
		}
	}
}
