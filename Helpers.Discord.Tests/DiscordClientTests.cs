using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Discord.Tests
{
	public class DiscordClientTests : IDisposable
	{
		private readonly IDiscordClient _client;
		private readonly System.Net.Http.HttpClient _httpClient;

		public DiscordClientTests()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			_httpClient = new System.Net.Http.HttpClient(handler)
			{
				BaseAddress = new Uri("https://ptb.discordapp.com/", UriKind.Absolute),
			};

			var httpClientFactoryMock = new Mock<IHttpClientFactory>();

			httpClientFactoryMock
				.Setup(f => f.CreateClient(It.IsAny<string>()))
				.Returns(_httpClient);

			var webhook = new Models.Webhook
			{
				Id = long.Parse(GetEnvironmentVariable("DiscordWebhookId")),
				Token = GetEnvironmentVariable("DiscordWebhookToken"),
			};

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

		private static string GetEnvironmentVariable(string variable)
		{
			string f(EnvironmentVariableTarget t) => Environment.GetEnvironmentVariable(variable, t);

			return f(EnvironmentVariableTarget.Process)
				?? f(EnvironmentVariableTarget.User)
				?? f(EnvironmentVariableTarget.Machine)
				?? throw new KeyNotFoundException($"Environment variable {variable} not found")
				{
					Data = { [nameof(variable)] = variable, },
				};
		}
	}
}
