using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Helpers.Identity.Tests;

public class EndToEndTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private readonly Uri _authority;
	private readonly string _clientId, _clientSecret, _scope;

	public EndToEndTests(Fixtures.ConfigurationFixture fixture)
	{
		_authority = fixture.Authority;
		_clientId = fixture.ClientId;
		_clientSecret = fixture.ClientSecret;
		_scope = fixture.Scope;
	}

	[Fact]
	public async Task StandUpTestApiTests_WithoutAuthorization_FailsWith401()
	{
		using var factory = new WebApplicationFactory<Helpers.Identity.Tests.TestApi.Program>();
		using var httpClient = factory.CreateClient();
		using var response = await httpClient.GetAsync("weatherforecast");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task DiscoveryDocumentTests()
	{
		var document = await GetDiscoveryDocumentAsync();
		Assert.NotNull(document);
		Assert.False(document.IsError, userMessage: document.Error);
	}

	[Fact]
	public async Task TokenTests()
	{
		var response = await GetTokenResponseAsync();
		Assert.NotNull(response);
		Assert.False(response.IsError, response.Error);
		Assert.Matches(@"^[-0-9A-Z_a-z]{160}\.[-0-9A-Z_a-z]{200,300}\.[-0-9A-Z_a-z]{300,400}$", response.AccessToken);
	}

	[Fact]
	public async Task EndToEndTests1()
	{
		var tokenResponse = await GetTokenResponseAsync();
		var token = tokenResponse.AccessToken;
		using var factory = new WebApplicationFactory<Helpers.Identity.Tests.TestApi.Program>();
		using var client = factory.CreateClient();
		using var request = new HttpRequestMessage(HttpMethod.Get, "weatherforecast");
		request.SetBearerToken(token);
		using var response = await client.SendAsync(request);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var s = await response.Content.ReadAsStringAsync();
		Assert.NotNull(s);
		Assert.NotEmpty(s);
		Assert.StartsWith("[", s);
	}

	private async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(CancellationToken cancellationToken = default)
	{
		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var client = new HttpClient(handler) { BaseAddress = _authority, };
		return await client.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken);
	}

	private async Task<TokenResponse> GetTokenResponseAsync(CancellationToken cancellationToken = default)
	{
		var disco = await GetDiscoveryDocumentAsync(cancellationToken);
		var tokenEndpoint = disco.TokenEndpoint;
		using var request = new ClientCredentialsTokenRequest
		{
			Address = tokenEndpoint,
			ClientId = _clientId,
			ClientSecret = _clientSecret,
			Scope = _scope,
		};

		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var client = new HttpClient(handler) { BaseAddress = _authority, };
		return await client.RequestClientCredentialsTokenAsync(request, cancellationToken);
	}
}
