using Moq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.HttpClient.Tests
{
	public class ClientTests : IDisposable
	{
		private readonly System.Net.Http.HttpClient _httpClient;
		private readonly IHttpClient _client;

		public ClientTests()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			_httpClient = new System.Net.Http.HttpClient(handler)
			{
				BaseAddress = new Uri("https://old.reddit.com/", UriKind.Absolute),
			};

			var clientFactoryMock = new Mock<IHttpClientFactory>();

			clientFactoryMock
				.Setup(f => f.CreateClient(It.Is<string>(name => name == nameof(HttpClient))))
				.Returns(_httpClient);

			_client = new Concrete.HttpClient(clientFactoryMock.Object);
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
			_client?.Dispose();
		}

		[Theory]
		[InlineData("/r/random")]
		public async Task ClientTests_WithBaseAddress(string uriString)
		{
			// Arrange
			var relativeUri = new Uri(uriString, UriKind.Relative);

			// Act
			var (statusCode, body, headers) = await _client.SendAsync(HttpMethod.Head, relativeUri);

			// Assert
			Assert.Equal(HttpStatusCode.Found, statusCode);
			Assert.NotNull(body);
			Assert.Equal(0, body.Length);
			Assert.True(headers.ContainsKey("Location"));
			Assert.NotEmpty(headers["Location"]);
			Assert.Single(headers["Location"]);
			Assert.Matches(
				@"^https:\/\/old\.reddit\.com\/r\/[_0-9A-Za-z]+\/\?utm_campaign=redirect&utm_medium=desktop&utm_source=reddit&utm_name=random_subreddit$",
				headers["Location"].Single());
		}

		[Theory]
		[InlineData("https://www.bing.com/")]
		public async Task ClientTests_WithoutBaseAddress(string uriString)
		{
			// Arrange
			var relativeUri = new Uri(uriString, UriKind.Absolute);

			// Act
			var (statusCode, body, headers) = await _client.SendAsync(HttpMethod.Get, relativeUri);

			// Assert
			Assert.Equal(HttpStatusCode.OK, statusCode);
			Assert.NotNull(body);
			Assert.NotEqual(0, body.Length);
		}
	}
}
