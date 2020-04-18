using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.HttpClient.Tests
{
	public sealed class ClientTests : IDisposable
	{
		private readonly System.Net.Http.HttpClient _httpClient;
		private readonly HttpClient _client;

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

			_client = new HttpClient(clientFactoryMock.Object);
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
			var (statusCode, stream, headers) = await _client.SendAsync(HttpMethod.Head, relativeUri);

			var body = await StreamToString(stream);

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
		[InlineData("https://cuts.diamond.mlb.com/FORGE/2019/2019-09/01/a352d164-5858748f-51ef2b5f-csvm-diamondx64-asset_1280x720_59_4000K.mp4")]
		public async Task ClientTests_WithoutBaseAddress(string uriString)
		{
			// Arrange
			var absoluteUri = new Uri(uriString, UriKind.Absolute);

			// Act
			var (statusCode, stream, _) = await _client.SendAsync(HttpMethod.Get, absoluteUri);

			var body = await StreamToString(stream);

			// Assert
			Assert.Equal(HttpStatusCode.OK, statusCode);
			Assert.NotNull(body);
			Assert.NotEqual(0, body.Length);
		}

		[Theory]
		[InlineData("https://minibeansjam.de/", typeof(HttpRequestException), "The SSL connection could not be established, see inner exception.")]
		[InlineData("https://puslelabs.ai/panelists", typeof(HttpRequestException), "No such host is known.")]
		public async Task ClientTests_Failing(string uriString, Type expectedException, string expectedMessage)
		{
			// Arrange
			var uri = new Uri(uriString, UriKind.Absolute);

			// Act
			try
			{
				await _client.SendAsync(HttpMethod.Get, uri);
				Assert.True(false);
			}
#pragma warning disable CA1031 // Do not catch general exception types
			catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
			{
				// Assert
				Assert.IsType(expectedException, exception);
				Assert.Equal(expectedMessage, exception.Message);
			}
		}

		private async static Task<string> StreamToString(Stream stream)
		{
			using var reader = new StreamReader(stream);

			return await reader.ReadToEndAsync();
		}

		[Theory]
		[InlineData("https://www.httpbin.org/get")]
		public async Task Test(string uriString)
		{
			// Arrange
			var uri = new Uri(uriString, UriKind.Absolute);
			using var client = new HttpClient();

			// Act
			var (statusCode, stream, headers) = await client.SendAsync(HttpMethod.Get, uri);
		}
	}

	public class HttpClient : HttpClientBase
	{
		public HttpClient(
			IHttpClientFactory httpClientFactory)
			: base(httpClientFactory)
		{ }

		public HttpClient(
			System.Net.Http.HttpClient httpClient)
			: base(httpClient)
		{ }

		public HttpClient() { }

		public async Task<(HttpStatusCode, Stream, IReadOnlyDictionary<string, IEnumerable<string>>)> SendAsync(
			HttpMethod httpMethod,
			Uri uri,
			string? body = default,
			[CallerMemberName] string? methodName = default)
		{
			var response = await base.SendAsync(httpMethod, uri, body, methodName);

			return (response.StatusCode!.Value, await response.TaskStream!, response.Headers!);
		}
	}
}
