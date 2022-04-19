using Xunit;

namespace Helpers.PhilipsHue.Tests;

public class PhilipsHueServiceTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly Services.IPhilipsHueService _sut;

	public PhilipsHueServiceTests(Fixtures.ClientFixture clientFixture)
	{
		_sut = new Services.Concrete.PhilipsHueService(clientFixture.Client);
	}

	[Theory]
	[InlineData("wall left")]
	[InlineData("wall right")]
	[InlineData("wall left", "wall right", "strip")]
	[InlineData("wall left", "strip")]
	public Task ToggleLight(params string[] names) => _sut.ToggleLightsAsync(names);

	[Theory]
	[InlineData("wall left", "wall right", "strip")]
	public async Task SetLightsOnBrightRed(params string[] names)
	{
		(_, var light) = await _sut.GetLightsByNamesAsync(names.First()).FirstAsync();

		var state = light.state.On().Bright().Red();

		await _sut.SetLightsStateAsync(state, names);
	}

	[Theory]
	[InlineData("wall left", "wall right", "strip")]
	public async Task SetLightsOnBrightWarm(params string[] names)
	{
		(_, var light) = await _sut.GetLightsByNamesAsync(names.First()).FirstAsync();

		var state = light.state.On().Bright().Warm();

		await _sut.SetLightsStateAsync(state, names);
	}

	[Theory]
	[InlineData("bedroom")]
	public Task ToggleLightsInGroups(params string[] groupNames) => _sut.ToggleLightsInGroupsAsync(groupNames);

	[Theory]
	[InlineData("bedroom", "main", true)]
	[InlineData("bedroom", "main", false)]
	public async Task SetLightState(string groupName, string lightName, bool on)
	{
		(_, var light) = await _sut.GetLightsByNamesAsync(lightName).FirstAsync();

		var state = light.state with { on = on, };

		await _sut.SetLightStateAsync(groupName, lightName, state);
	}

	[Theory]
	[InlineData("bedroom", "main")]
	public async Task SetBedroomMainLightOnColdestBrightest(string groupName, string lightName)
	{
		(_, var light) = await _sut.GetLightsByNamesAsync(lightName).FirstAsync();

		var state = light.state.On().Coldest().Brightest();

		await _sut.SetLightStateAsync(groupName, lightName, state);
	}
}
