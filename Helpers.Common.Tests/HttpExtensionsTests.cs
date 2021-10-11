using Xunit;

namespace Helpers.Common.Tests;

public class HttpExtensionsTests : IClassFixture<Fixtures.HttpClientFixture>
{
	private readonly HttpClient _httpClient;

	public HttpExtensionsTests(Fixtures.HttpClientFixture fixture)
	{
		_httpClient = fixture.HttpClient;
	}

	[Theory]
	[InlineData("https://www.httpbin.org/get")]
	public async Task HttpBinResponse(string uriString)
	{
		// Arrange
		var uri = new Uri(uriString);

		// Act
		var response = await _httpClient.GetAsync<HttpBinResponseObject>(uri);

		// Assert
		Assert.NotNull(response);
		Assert.Equal(uriString, response.url);
		Assert.NotNull(response.headers);
		Assert.NotEmpty(response.headers!);
	}
}

public class HttpBinResponseObject
{
#pragma warning disable IDE1006 // Naming Styles
	public Dictionary<string, string>? args { get; set; }
	public Dictionary<string, string>? headers { get; set; }
	public string? origin { get; set; }
	public string? url { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
