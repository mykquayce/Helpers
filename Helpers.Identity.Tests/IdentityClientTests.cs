using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;

namespace Helpers.Identity.Tests;

public class IdentityClientTests
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Fact(Skip = "needs third party")]
	public async Task EndToEndTests()
	{
		var client = new HttpClient();

		var disco = await client.GetDiscoveryDocumentAsync("https://identityserver");
		Assert.False(disco.IsError, userMessage: $"{disco.ErrorType}: {disco.Error}");

		// request token
		var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
		{
			Address = disco.TokenEndpoint,
			ClientId = "client",
			ClientSecret = "secret",

			Scope = "api1"
		});

		Assert.False(tokenResponse.IsError, userMessage: $"{tokenResponse.ErrorType} {tokenResponse.Error} ({tokenResponse.ErrorDescription})");

		Console.WriteLine(tokenResponse.Json);
		Console.WriteLine("\n\n");

		// call api
		var apiClient = new HttpClient();
		apiClient.SetBearerToken(tokenResponse.AccessToken);

		var response = await apiClient.GetAsync("https://api:6001/identity");

		Assert.True(response.IsSuccessStatusCode);
		var content = await response.Content.ReadAsStringAsync();
		Assert.NotNull(content);
		Assert.NotEmpty(content);
		Assert.StartsWith("[", content);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Theory(Skip = "needs third party")]
	[InlineData("https://identityserver")]
	public async Task GetDiscoveryDocumentTests(string baseAddressString)
	{
		DiscoveryDocumentResponse disco;
		{
			using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			using var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddressString, UriKind.Absolute), };
			disco = await client.GetDiscoveryDocumentAsync();
		}

		Assert.False(disco.IsError, userMessage: $"{disco.ErrorType}: {disco.Error}");
		Assert.NotNull(disco.TokenEndpoint);
		Assert.NotEmpty(disco.TokenEndpoint);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Theory(Skip = "needs third party")]
	[InlineData(10, "https://identityserver", "client", "secret", "api1")]
	public async Task GetAccessTokenTests(int count, string endpointString, string clientId, string clientSecret, string scope)
	{
		ICollection<string> accessTokens = new List<string>();

		{
			var config = new Config(new Uri(endpointString), clientId, clientSecret, scope);
			using var httpClient = new HttpClient
			{
				BaseAddress = config.Authority,
			};
			using var memoryCache = new MemoryCache(new MemoryCacheOptions());
			Identity.Clients.IIdentityClient client = new Helpers.Identity.Clients.Concrete.IdentityClient(config, httpClient, memoryCache);

			while (count-- > 0)
			{
				var accessToken = await client.GetAccessTokenAsync();
				accessTokens.Add(accessToken);
			}
		}

		Assert.NotEmpty(accessTokens);
		Assert.All(accessTokens, Assert.NotNull);
		Assert.All(accessTokens, Assert.NotEmpty);
		Assert.All(accessTokens, s => Assert.Matches(@"^[0-9A-Za-z]{160}\.[0-9A-Za-z]{216}\.[\-0-9A-Z_a-z]{342}$", s));
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "needs third party")]
	[Theory(Skip = "needs third party")]
	[InlineData("https://identityserver", "client", "secret", "api1")]
	public async Task UseAccessTokenTests(string endpointString, string clientId, string clientSecret, string scope)
	{
		string response;
		{
			string accessToken;
			{
				var config = new Config(new Uri(endpointString), clientId, clientSecret, scope);
				using var httpClientHander = new HttpClientHandler { AllowAutoRedirect = false, };
				using var httpClient = new HttpClient(httpClientHander) { BaseAddress = config.Authority, };
				using var memoryCache = new MemoryCache(new MemoryCacheOptions());
				Identity.Clients.IIdentityClient client = new Helpers.Identity.Clients.Concrete.IdentityClient(config, httpClient, memoryCache);
				accessToken = await client.GetAccessTokenAsync();
			}
			{
				using var httpClientHander = new HttpClientHandler { AllowAutoRedirect = false, };
				using var httpClient = new HttpClient(httpClientHander) { BaseAddress = new Uri("https://api:6001"), };
				httpClient.SetBearerToken(accessToken);

				response = await httpClient.GetStringAsync("/identity");
			}
		}
		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("[", response);
	}
}
