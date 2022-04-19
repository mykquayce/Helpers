using Xunit;

namespace Helpers.Elgato.Tests;

public sealed class ElgatoClientTests : IClassFixture<Fixtures.ElgatoClientFixture>
{
	private readonly IElgatoClient _sut;

	public ElgatoClientTests(
		Fixtures.ElgatoClientFixture clientFixture)
	{
		_sut = clientFixture.Client;
	}

	[Fact]
	public async Task GetAccessoryInfo()
	{
		var info = await _sut.GetAccessoryInfoAsync();

		Assert.NotNull(info);

		Assert.NotEmpty(info.productName);
		Assert.InRange(info.hardwareBoardType, 1, int.MaxValue);
		Assert.InRange(info.firmwareBuildNumber, 1, int.MaxValue);
		Assert.NotNull(info.firmwareVersion);
		Assert.NotEmpty(info.serialNumber);
		Assert.NotEmpty(info.displayName);
		Assert.NotEmpty(info.features);
		Assert.All(info.features, Assert.NotNull);
		Assert.All(info.features, Assert.NotEmpty);
	}

	[Fact]
	public async Task GetLight()
	{
		var light = await _sut.GetLightAsync().FirstAsync();

		Assert.NotNull(light);
		Assert.InRange(light.brightness, 0, 100);
		Assert.InRange(light.on, 0, 1);
		Assert.InRange(light.temperature, 140, 350);
	}

	[Theory]
	[InlineData(1, 23, 343)]
	[InlineData(0, 23, 343)]
	public async Task SetLight(byte on, byte brightness, short temperature)
	{
		var before = new Models.MessageObject.LightObject(on, brightness, temperature);

		await _sut.SetLightAsync(before);

		var after = await _sut.GetLightAsync().FirstAsync();

		Assert.Equal(on, after.on);
		Assert.Equal(brightness, after.brightness);
		Assert.Equal(temperature, after.temperature);
	}
}
