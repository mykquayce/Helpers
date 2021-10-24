using Helpers.Web.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Web.Tests
{
	public sealed class WebClientBaseTests : IDisposable
	{
		private readonly HttpClient _httpClient;
		private readonly WebClient _client;

		public WebClientBaseTests()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			_httpClient = new HttpClient(handler)
			{
				BaseAddress = new Uri("https://old.reddit.com/", UriKind.Absolute),
			};

			_client = new WebClient(_httpClient);
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
		}

		[Theory]
		[InlineData("/r/random")]
		public async Task ClientTests_WithBaseAddress(string uriString)
		{
			// Arrange
			var relativeUri = new Uri(uriString, UriKind.Relative);

			// Act
			var response = await _client.SendAsync(HttpMethod.Head, relativeUri);

			Assert.NotNull(response);
			Assert.NotNull(response.TaskStream);

			var body = await StreamToString(await response.TaskStream!);

			// Assert
			Assert.Equal(HttpStatusCode.Found, response.StatusCode);
			Assert.NotNull(body);
			Assert.Equal(0, body.Length);
			Assert.NotNull(response.Headers);
			Assert.True(response.Headers!.ContainsKey("Location"));
			Assert.NotEmpty(response.Headers["Location"]);
			Assert.Single(response.Headers["Location"]);
			Assert.Matches(
				@"^https:\/\/old\.reddit\.com\/r\/[_0-9A-Za-z]+\/\?utm_campaign=redirect&utm_medium=desktop&utm_source=reddit&utm_name=random_subreddit$",
				response.Headers["Location"].Single());
		}

		[Theory]
		[InlineData("https://www.bing.com/")]
		[InlineData("https://cuts.diamond.mlb.com/FORGE/2019/2019-09/01/a352d164-5858748f-51ef2b5f-csvm-diamondx64-asset_1280x720_59_4000K.mp4")]
		public async Task ClientTests_WithoutBaseAddress(string uriString)
		{
			// Arrange
			var absoluteUri = new Uri(uriString, UriKind.Absolute);

			// Act
			var response = await _client.SendAsync(HttpMethod.Get, absoluteUri);

			var body = await StreamToString(await response.TaskStream!);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.NotNull(body);
			Assert.NotEqual(0, body.Length);
		}

		[Theory]
		[InlineData("https://minibeansjam.de/", typeof(HttpRequestException), "The SSL connection could not be established, see inner exception.")]
		[InlineData("https://puslelabs.ai/panelists", typeof(HttpRequestException), "No such host is known. (puslelabs.ai:443)")]
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
			catch (Exception exception)
			{
				// Assert
				Assert.IsType(expectedException, exception);
				Assert.Equal(expectedMessage, exception.Message);
				Assert.Contains("RequestUri", exception.Data.Keys.Cast<object>());
				Assert.Equal(uriString, exception.Data["RequestUri"]);
			}
		}

		private async static Task<string> StreamToString(Stream stream)
		{
			using var reader = new StreamReader(stream);

			return await reader.ReadToEndAsync();
		}

		[Theory]
		[InlineData("https://www.httpbin.org/get")]
		public async Task DeserializeToString(string uriString)
		{
			// Arrange
			var uri = new Uri(uriString, UriKind.Absolute);
			var client = new WebClient();

			// Act
			var response = await client.SendAsync<string>(HttpMethod.Get, uri);

			// Assert
			Assert.NotNull(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.NotNull(response.Object);
			Assert.NotEmpty(response.Object);
			Assert.StartsWith("{", response.Object);

		}
	}

	public class WebClient : WebClientBase
	{
		public WebClient(HttpClient httpClient)
			: base(httpClient)
		{ }

		public WebClient() { }

		public Task<IResponse> SendAsync(HttpMethod httpMethod, Uri uri, string? body = default)
			=> base.SendAsync(httpMethod, uri, body);

		public Task<IResponse<T>> SendAsync<T>(HttpMethod httpMethod, Uri uri, string? body = default)
			where T : class
			=> base.SendAsync<T>(httpMethod, uri, body);

	}
}
