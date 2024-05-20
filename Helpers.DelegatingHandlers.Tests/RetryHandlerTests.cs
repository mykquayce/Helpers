using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net;

namespace Helpers.DelegatingHandlers.Tests;

public class RetryHandlerTests(RetryHandlerTests.Fixture fixture) : IClassFixture<RetryHandlerTests.Fixture>
{
	private readonly TestClient _client = fixture.TestClient;

	[Theory]
	[InlineData(HttpStatusCode.OK, 0, 300)]
	[InlineData(HttpStatusCode.MovedPermanently, 0, 300)]
	[InlineData(HttpStatusCode.Found, 0, 300)]
	[InlineData(HttpStatusCode.NotModified, 0, 300)]
	[InlineData(HttpStatusCode.TemporaryRedirect, 0, 300)]
	[InlineData(HttpStatusCode.PermanentRedirect, 0, 300)]
	[InlineData(HttpStatusCode.TooManyRequests, 3_900, 4_100)]
	[InlineData(HttpStatusCode.ServiceUnavailable, 3_900, 4_100)]
	public async Task Test1(HttpStatusCode statusCode, int min, int max)
	{
		// Act
		var stopwatch = Stopwatch.StartNew();
		var response = await _client.GetStatusCodeAsync(statusCode);
		stopwatch.Stop();

		// Assert
		Assert.Equal(statusCode, response.StatusCode);
		Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, min, max);
	}

	public sealed class Fixture : IDisposable
	{
		private readonly IServiceProvider _serviceProvider;

		public Fixture()
		{
			var initialData = new Dictionary<string, string?>
			{
				["count"] = "2",
				["pause"] = "00:00:02",
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();

			var services = new ServiceCollection();

			services
				.Configure<RetryHandler.Config>(configuration);

			services
				.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
				.AddTransient<MockingDelegatingHandler>()
				.AddTransient<RetryHandler>();

			services
				.AddHttpClient<TestClient>(c => c.BaseAddress = new Uri("http://localhost/"))
					.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
					.AddHttpMessageHandler<RetryHandler>()
					.AddHttpMessageHandler<MockingDelegatingHandler>();

			_serviceProvider = services
				.BuildServiceProvider();

			TestClient = _serviceProvider.GetRequiredService<TestClient>();
		}

		public TestClient TestClient { get; }

		public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
	}

	public class TestClient(HttpClient httpClient)
	{
		public Task<HttpResponseMessage> GetStatusCodeAsync(HttpStatusCode statusCode, CancellationToken cancellationToken = default)
			=> httpClient.GetAsync($"status/{statusCode:D}", cancellationToken);
	}

	private class MockingDelegatingHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var query = request.RequestUri?.PathAndQuery;

			if (query?.StartsWith("/status/", StringComparison.OrdinalIgnoreCase) ?? false)
			{
				var code = (HttpStatusCode)int.Parse(query[8..]);
				var response = new HttpResponseMessage(code);
				return Task.FromResult(response);
			}

			return base.SendAsync(request, cancellationToken);
		}
	}
}
