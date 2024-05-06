using Microsoft.Extensions.DependencyInjection;

namespace Helpers.DelegatingHandlers.Tests;

public class UserAgentHandlerTests
{
	[Theory, InlineData("Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405")]
	public async Task Test1(string userAgent)
	{
		// Arrange
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddUserAgentHandler(userAgent)
			.AddTransient<MockingHandler>()
			.AddHttpClient<Client>()
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<UserAgentHandler>()
				.AddHttpMessageHandler<MockingHandler>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<Client>();

		// Act
		var response = await sut.GetAsync();

		// Assert
		Assert.Equal(userAgent, response.RequestMessage?.Headers.UserAgent.ToString());
	}

	private class Client(HttpClient httpClient)
	{
		public Task<HttpResponseMessage> GetAsync() => httpClient.GetAsync("http://localhost/");
	}

	private class MockingHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = new HttpResponseMessage { RequestMessage = request, };
			return Task.FromResult(response);
		}
	}
}
