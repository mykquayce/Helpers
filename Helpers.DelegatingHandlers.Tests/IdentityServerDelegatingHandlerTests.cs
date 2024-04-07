using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.DelegatingHandlers.Tests;

public class IdentityServerDelegatingHandlerTests(IdentityServerDelegatingHandlerTests.Fixture fixture)
	: IClassFixture<IdentityServerDelegatingHandlerTests.Fixture>
{
	private readonly TestClient _sut = fixture.TestClient;

	[Fact]
	public async Task Test1()
	{
		// Act
		var headers = await _sut.GetHeadersAsync()
			.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

		// Assert
		Assert.NotEmpty(headers);
		Assert.DoesNotContain(default, headers.Keys);
		Assert.Contains(headers.Keys, filter: s => string.Equals(s, "authorization", StringComparison.OrdinalIgnoreCase));
		Assert.NotEmpty(headers["authorization"]);
		Assert.Single(headers["authorization"]);
		Assert.NotEmpty(headers["authorization"][0]);
		Assert.StartsWith("Bearer eyJ", headers["authorization"][0]);
	}

	public sealed class Fixture : IDisposable
	{
		private readonly static Uri _authority = new("https://identityserver/");
		private const string _clientId = "client", _clientSecret = "secret", _scope = "api1";

		private readonly IServiceProvider _provider;

		public Fixture()
		{
			var initialData = new Dictionary<string, string?>
			{
				["authority"] = _authority.OriginalString,
				["clientId"] = _clientId,
				["clientSecret"] = _clientSecret,
				["scope"] = _scope,
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();

			_provider = new ServiceCollection()
				.AddMemoryCache()
				.Configure<IdentityServerDelegatingHandler.Config>(configuration)
				.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
				.AddTransient<MockingHandler>()
				.AddTransient<CachingHandler>()
				.AddHttpClient<IdentityServerDelegatingHandler>((provider, client) =>
				{
					var config = provider.GetRequiredService<IOptions<IdentityServerDelegatingHandler.Config>>().Value;
					client.BaseAddress = config.Authority;
				})
					.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
					.AddHttpMessageHandler<CachingHandler>()
					.Services
				.AddHttpClient<TestClient>(c => c.BaseAddress = new Uri("http://localhost/"))
					.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
					.AddHttpMessageHandler<IdentityServerDelegatingHandler>()
					.AddHttpMessageHandler<MockingHandler>()
					.Services
				.BuildServiceProvider();

			TestClient = _provider.GetRequiredService<TestClient>();
		}

		public TestClient TestClient { get; }

		public void Dispose() => (_provider as ServiceProvider)?.Dispose();
	}
}
