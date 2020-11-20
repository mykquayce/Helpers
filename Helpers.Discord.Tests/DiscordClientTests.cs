using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Discord.Tests
{
	public class DiscordClientTests :
		IClassFixture<Helpers.XUnitClassFixtures.HttpClientFixture>,
		IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
	{
		private const string _uriString = "https://ptb.discordapp.com/";
		private readonly IDiscordClient _sut;

		public DiscordClientTests(
			Helpers.XUnitClassFixtures.HttpClientFixture httpClientFixture,
			Helpers.XUnitClassFixtures.UserSecretsFixture userSecretsFixture)
		{
			var httpClient = httpClientFixture.HttpClient;

			if (httpClient.BaseAddress is null)
			{
				httpClient.BaseAddress = new Uri(_uriString);
			}

			var httpClientFactoryMock = new Mock<IHttpClientFactory>();

			httpClientFactoryMock
				.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(httpClient);

			var webHook = new Models.Webhook
			{
				Id = long.TryParse(userSecretsFixture["Discord:WebHook:Id"], out var l)
					? l
					: throw new ArgumentNullException("Discord:WebHook:Id"),
				Token = userSecretsFixture["Discord:WebHook:Token"]
					?? throw new ArgumentNullException("Discord:WebHook:Token"),
			};

			var webhookOptions = Options.Create(webHook);

			_sut = new Helpers.Discord.Concrete.DiscordClient(
				httpClientFactoryMock.Object,
				webhookOptions);
		}

		[Theory]
		[InlineData("Hello world")]
		[InlineData("Hello\r\nworld")]
		public Task DiscordClientTests_SendMessageAsync(string message) => _sut.SendMessageAsync(message);
	}
}
