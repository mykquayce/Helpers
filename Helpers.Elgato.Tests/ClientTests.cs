using Helpers.Elgato.Models;
using Xunit;

namespace Helpers.Elgato.Tests;

public class ClientTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IWhiteLightClient _sut = fixture.WhiteLightClient;

	[Fact]
	public async Task GetInfoTests()
	{
		// Act
		var info = await _sut.GetInfoAsync();

		// Assert
		Assert.NotEqual(default, info);
		Assert.NotEmpty(info.productName);
		Assert.NotEmpty(info.serialNumber);
	}

	[Fact]
	public async Task GetTests()
	{
		// Act
		var light = await _sut.GetAsync();

		// Assert
		Assert.NotEqual(default, light);
		Assert.NotEqual(default, light.brightness);
	}

	[Theory, InlineData(1, 13, 317)]
	public async Task SetTests(byte on, byte brightness, short temperature)
	{
		// Act
		var light = new WhiteLight(on, brightness, temperature);
		var response = await _sut.SetAsync(light);
		var content = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.True(response.IsSuccessStatusCode, response.StatusCode + " " + content);
	}
}
