namespace Helpers.NetworkDiscovery.Tests;

public class PhysicalAddressResolverTests(Fixtures.Fixture fixture) : IClassFixture<Fixtures.Fixture>
{
	private readonly TestClient _sut = fixture.TestClient;

	[Theory]
	[InlineData("http://3c6a9d14d765:9123/elgato/accessory-info")]
	public async Task Test1(string requestUri)
	{
		var response = await _sut.GetAsync(requestUri);
		var content = await response.Content.ReadAsStringAsync();

		Assert.True(response.IsSuccessStatusCode, response.StatusCode + " " + content);
		Assert.NotEmpty(content);
		Assert.StartsWith("{", content);
		Assert.NotEqual("{}", content);
	}
}
