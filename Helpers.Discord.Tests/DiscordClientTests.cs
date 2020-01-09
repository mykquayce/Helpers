using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Discord.Tests
{
	public sealed class DiscordClientTests : IDisposable
	{
		private readonly IDiscordClient _client;
		private readonly System.Net.Http.HttpClient _httpClient;

		public DiscordClientTests()
		{
			var config = new ConfigurationBuilder()
				.AddUserSecrets("8093bdd7-e455-40d8-a574-90ff004106b6")
				.Build();

			var webhook = config.GetSection("Discord").GetSection(nameof(Models.Webhook)).Get<Models.Webhook>();

			var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			_httpClient = new System.Net.Http.HttpClient(handler)
			{
				BaseAddress = new Uri("https://ptb.discordapp.com/", UriKind.Absolute),
			};

			var httpClientFactoryMock = new Mock<IHttpClientFactory>();

			httpClientFactoryMock
				.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(_httpClient);

			var webhookOptions = Mock.Of<IOptions<Models.Webhook>>(o => o.Value == webhook);

			_client = new Helpers.Discord.Concrete.DiscordClient(
				httpClientFactoryMock.Object,
				webhookOptions);
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
			_client?.Dispose();
		}

		[Theory]
		[InlineData("Hello world")]
		public Task DiscordClientTests_SendMessageAsync(string message) => _client.SendMessageAsync(message);
	}
}
