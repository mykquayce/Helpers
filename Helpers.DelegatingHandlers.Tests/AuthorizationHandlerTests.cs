using Microsoft.Extensions.DependencyInjection;

namespace Helpers.DelegatingHandlers.Tests;

public class AuthorizationHandlerTests
{
	[Fact]
	public async Task Test1()
	{
		// Arrange
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddTransient<AuthorizationHandler>()
			.AddTransient<MockingHandler>()
			.AddTransient<AuthorizationHandler.ITokenGetter, AuthorizationClient>()
			.AddHttpClient<TestClient>(c => c.BaseAddress = new Uri("http://localhost/"))
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<AuthorizationHandler>()
				.AddHttpMessageHandler<MockingHandler>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<TestClient>();

		// Act
		var headers = await sut.GetHeadersAsync()
			.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

		// Assert
		Assert.NotEmpty(headers);
		Assert.Contains("Authorization", headers.Keys);
		Assert.NotNull(headers["Authorization"]);
		Assert.NotEmpty(headers["Authorization"]);
		Assert.Single(headers["Authorization"]);
		Assert.NotNull(headers["Authorization"].First());
		Assert.StartsWith("Bearer ", headers["Authorization"].First(), StringComparison.OrdinalIgnoreCase);
	}

	private class AuthorizationClient : AuthorizationHandler.ITokenGetter
	{
		public ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default)
		{
			return ValueTask.FromResult("token");
		}
	}
}
