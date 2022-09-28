using System.Drawing;
using System.Text.Json;

namespace Helpers.PhilipsHue.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinition))]
public class ClientTests : IClassFixture<Fixtures.Fixture>
{
	private readonly Config _config;
	private readonly IClient _client;

	public ClientTests(Fixtures.Fixture fixture)
	{
		_config = fixture.Config;
		_client = fixture.Client;
	}

	[Fact]
	public async Task GetLightsAliasesTests()
	{
		var kvps = await _client.GetLightAliasesAsync(baseAddress: null).ToListAsync();

		Assert.Distinct(kvps.Select(kvp => kvp.Key), StringComparer.OrdinalIgnoreCase);

		IDictionary<string, int> actual = kvps
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
		Assert.All(actual.Keys, Assert.NotNull);
		Assert.All(actual.Keys, Assert.NotEmpty);
		Assert.All(actual.Values, id => Assert.InRange(id, 1, int.MaxValue));
	}

	[Theory]
	[InlineData(3)]
	public async Task GetLightColorTests(int index)
	{
		var actual = await _client.GetLightColorAsync(index, baseAddress: null);

		Assert.NotEqual(default, actual);
		Assert.NotNull(actual.Name);
	}

	[Theory]
	[InlineData(4, 0, 0, 128)]
	[InlineData(4, 0, 0, 255)]
	[InlineData(4, 0, 0, 64)]
	[InlineData(4, 0, 128, 0)]
	[InlineData(4, 128, 0, 0)]
	public async Task SetLightColorTests(int index, int red, int green, int blue)
	{
		// Arrange
		var comparer = new Comparers.ColorComparer(tolerance: 10);
		var color = Color.FromArgb(red, green, blue);

		// Act
		await _client.SetLightColorAsync(index, color, baseAddress: null);
		var after = await _client.GetLightColorAsync(index, baseAddress: null);

		// Assert
		Assert.Equal(color, after, comparer);
	}

	[Theory]
	[InlineData(4)]
	public Task GetLightPowerTests(int index)
	{
		return _client.GetLightPowerAsync(index, baseAddress: null);
	}

	[Theory]
	[InlineData(4, false)]
	[InlineData(4, true)]
	public Task SetLightPowerTests(int index, bool on)
	{
		return _client.SetLightPowerAsync(index, on, baseAddress: null);
	}

	[Theory]
	[InlineData(4)]
	public async Task GetLightBrightnessTests(int index)
	{
		var actual = await _client.GetLightBrightnessAsync(index, baseAddress: null);
		Assert.InRange(actual, 0, 1);
	}

	[Theory]
	[InlineData(4, .8f)]
	[InlineData(4, .4f)]
	public Task SetLightBrightnessTests(int index, float brightness)
	{
		return _client.SetLightBrightnessAsync(index, brightness, baseAddress: null);
	}

	[Theory]
	[InlineData(4)]
	public async Task GetLightTemperatureTests(int index)
	{
		var actual = await _client.GetLightTemperatureAsync(index, baseAddress: null);
		Assert.InRange(actual, 2_900, 7_000);
	}

	[Theory]
	[InlineData(4, 2_900)]
	[InlineData(4, 7_000)]
	public Task SetLightTemperatureTests(int index, short brightness)
	{
		return _client.SetLightTemperatureAsync(index, brightness, baseAddress: null);
	}

	[Theory]
	[InlineData(4)]
	public async Task BaseAddressTests(int index)
	{
		using var httpHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var httpClient = new HttpClient(httpHandler) { BaseAddress = null, };
		var config = new Config(PhysicalAddress: null, HostName: null!, Username: _config.Username);

		IClient sut = new Concrete.Client(config, httpClient, JsonSerializerOptions.Default);

		var actual = await sut.GetLightColorAsync(index, baseAddress: _config.BaseAddress);

		Assert.NotEqual(default, actual);
	}

	[Theory]
	[InlineData("http", "localhost")]
	public void BaseAddressAlwaysEndsWithSlash_UriBuilder(string schemeName, string hostName)
	{
		var uri = new UriBuilder(schemeName, hostName).Uri;
		Assert.EndsWith("/", uri.ToString());
	}

	[Theory]
	[InlineData("http://localhost")]
	[InlineData("http://localhost/")]
	public void BaseAddressAlwaysEndsWithSlash_UriConstructor(string uriString)
	{
		var uri = new Uri(uriString);
		Assert.EndsWith("/", uri.ToString());
	}
}
