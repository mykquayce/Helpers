using Helpers.PhilipsHue.Clients;
using Xunit;

namespace Helpers.PhilipsHue.Tests;

public sealed class PhilipsHueClientTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly IPhilipsHueClient _sut;

	public PhilipsHueClientTests(Fixtures.ClientFixture clientFixture)
	{
		_sut = clientFixture.Client;
	}

	[Fact]
	public async Task GetAll()
	{
		var all = await _sut.GetAllAsync();

		Assert.NotNull(all);
		Assert.NotNull(all.config);
	}

	[Theory]
	[InlineData("3")]
	public async Task<Models.LightObject> GetLight(string id)
	{
		var light = await _sut.GetLightAsync(id);

		Assert.NotNull(light);
		Assert.NotNull(light.state);

		return light;
	}

	[Fact]
	public async Task GetLights()
	{
		var lights = await _sut.GetLightsAsync().ToListAsync();

		Assert.NotNull(lights);
		Assert.NotEmpty(lights);

		foreach (var (id, light) in lights)
		{
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.NotNull(light);
			Assert.NotNull(light.state);
		}
	}

	[Theory]
	[InlineData("3", true)]
	[InlineData("3", false)]
	public async Task SetLightState(string id, bool on)
	{
		var light = await GetLight(id);
		var state = light.state with { on = on, };

		await _sut.SetLightStateAsync(id, state);
	}

	[Theory]
	[InlineData("3")]
	public async Task BrightestThenOff(string id)
	{
		var light = await GetLight(id);
		{
			var brightest = light.state.On().Brightest();
			await _sut.SetLightStateAsync(id, brightest);
		}
		await Task.Delay(millisecondsDelay: 2_000);
		{
			var off = light.state.Off();
			await _sut.SetLightStateAsync(id, off);
		}
	}

	[Theory]
	[InlineData("1")]
	public async Task GetGroup(string id)
	{
		var group = await _sut.GetGroupAsync(id);

		Assert.NotNull(group);
		Assert.NotNull(group.name);
		Assert.NotEmpty(group.name);
		Assert.NotNull(group.lights);
		Assert.NotEmpty(group.lights);
		Assert.DoesNotContain(default, group.lights);
		Assert.All(group.lights, Assert.NotNull);
		Assert.All(group.lights, Assert.NotEmpty);
	}

	[Fact]
	public async Task GetGroups()
	{
		var groups = await _sut.GetGroupsAsync().ToListAsync();

		Assert.NotNull(groups);
		Assert.NotEmpty(groups);

		foreach (var (id, group) in groups)
		{
			Assert.NotNull(id);
			Assert.NotEmpty(id);
			Assert.NotNull(group);
			Assert.NotNull(group.name);
			Assert.NotNull(group.lights);
			Assert.NotEmpty(group.lights);
			Assert.DoesNotContain(default, group.lights);
			Assert.All(group.lights, Assert.NotNull);
			Assert.All(group.lights, Assert.NotEmpty);
		}
	}

	[Fact]
	public async Task GetConfig()
	{
		var config = await _sut.GetConfigAsync();

		Assert.NotNull(config);

		Assert.NotNull(config.name);
		Assert.NotNull(config.mac);
		Assert.NotNull(config.ipaddress);

		Assert.NotEmpty(config.name);
		Assert.NotEmpty(config.mac);
		Assert.NotEmpty(config.ipaddress);
	}

	[Theory]
	[InlineData("1")]
	[InlineData("2")]
	[InlineData("3")]
	[InlineData("4")]
	[InlineData("5")]
	[InlineData("6")]
	[InlineData("7")]
	[InlineData("8")]
	[InlineData("9")]
	[InlineData("10")]
	[InlineData("11")]
	[InlineData("12")]
	[InlineData("13")]
	public async Task GetRule(string id)
	{
		var rule = await _sut.GetRuleAsync(id);

		Assert.NotNull(rule);
	}
}
