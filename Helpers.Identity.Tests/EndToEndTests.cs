using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

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
	public async Task<string> DiscoveryDocumentTests()
	{
		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var client = new HttpClient(handler) { BaseAddress = _authority, };
		var document = await client.GetDiscoveryDocumentAsync();
		Assert.NotNull(document);
		Assert.False(document.IsError, userMessage: document.Error);
		return document.TokenEndpoint;
	}

	[Fact]
	public async Task<string> TokenTests()
	{
		var tokenEndpoint = await DiscoveryDocumentTests();
		using var request = new ClientCredentialsTokenRequest
		{
			Address = tokenEndpoint,
			ClientId = _clientId,
			ClientSecret = _clientSecret,
			Scope = _scope,
		};

		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var client = new HttpClient(handler) { BaseAddress = _authority, };
		var response = await client.RequestClientCredentialsTokenAsync(request);
		Assert.NotNull(response);
		Assert.False(response.IsError, response.Error);
		Assert.Matches(@"^[-0-9A-Z_a-z]{160}\.[-0-9A-Z_a-z]{216}\.[-0-9A-Z_a-z]{342}$", response.AccessToken);
		return response.AccessToken;
	}

	[Fact]
	public async Task EndToEndTests1()
	{
		var token = await TokenTests();
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
}
