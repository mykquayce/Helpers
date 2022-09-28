using System.Drawing;

namespace Helpers.PhilipsHue.Tests;

public class ServiceTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IService _service;

	public ServiceTests(Fixtures.Fixture fixture)
	{
		_service = fixture.Service;
	}

	[Fact]
	public async Task GetLightsAliasesTests()
	{
		var actual = await _service.GetLightAliasesAsync().ToListAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
		Assert.All(actual, Assert.NotNull);
		Assert.All(actual, Assert.NotEmpty);
	}

	[Theory]
	[InlineData("wall right")]
	public Task GetLightPowerTests(string alias)
	{
		return _service.GetLightPowerAsync(alias);
	}

	[Theory]
	[InlineData("wall right", false)]
	[InlineData("wall right", true)]
	public Task SetLightPowerTests(string alias, bool on)
	{
		return _service.SetLightPowerAsync(alias, on);
	}

	[Theory]
	[InlineData("wall right")]
	public async Task GetLightBrightnessTests(string alias)
	{
		var actual = await _service.GetLightBrightnessAsync(alias);
		Assert.InRange(actual, 0, 1);
	}

	[Theory]
	[InlineData("wall right", .8f)]
	[InlineData("wall right", .4f)]
	public Task SetLightBrightnessTests(string alias, float brightness)
	{
		return _service.SetLightBrightnessAsync(alias, brightness);
	}

	[Theory]
	[InlineData("wall right")]
	public async Task GetLightTemperatureTests(string alias)
	{
		 var actual = await _service.GetLightTemperatureAsync(alias);
		Assert.InRange(actual, 2_900, 7_000);
	}

	[Theory]
	[InlineData("wall right", 2_900)]
	[InlineData("wall right", 7_000)]
	public Task SetLightTemperatureTests(string alias, short brightness)
	{
		return _service.SetLightTemperatureAsync(alias, brightness);
	}

	[Theory]
	[InlineData("wall right")]
	public async Task GetLightColorTests(string alias)
	{
		var actual = await _service.GetLightColorAsync(alias);

		Assert.NotEqual(default, actual);
		Assert.NotNull(actual.Name);
	}

	[Theory]
	[InlineData("wall right", 0, 0, 128)]
	[InlineData("wall right", 0, 128, 0)]
	[InlineData("wall right", 128, 0, 0)]
	public Task SetLightColorTests(string alias, int red, int green, int blue)
	{
		var color = Color.FromArgb(red, green, blue);
		return _service.SetLightColorAsync(alias, color);
	}
}
