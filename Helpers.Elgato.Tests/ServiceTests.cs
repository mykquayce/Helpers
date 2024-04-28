using System.Drawing;
using System.Text.Json;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(NonParallelCollectionDefinitionClass))]
public class ServiceTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IWhiteLightService _sut = fixture.WhiteLightService;

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task SetPowerStateTests(bool on)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);
		await _sut.SetPowerStateAsync(on, cts.Token);
	}

	[Theory]
	[InlineData(8)]
	public async Task TogglePowerStateTests(int count)
	{
		ICollection<bool> states = [];
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);

		while (count-- > 0)
		{
			await _sut.TogglePowerStateAsync(cts.Token);
			var light = await _sut.GetAsync(cts.Token);
			var state = light.on == 1;
			states.Add(state);
		}

		Assert.NotEmpty(states.Where(b => b));
		Assert.NotEmpty(states.Where(b => !b));
	}

	[Theory]
	[InlineData(20)]
	[InlineData(40)]
	[InlineData(60)]
	public async Task SetBrightnessTests(byte brightness)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);

		await _sut.SetBrightnessAsync(brightness, cts.Token);
		var light = await _sut.GetAsync(cts.Token);

		Assert.Equal(brightness, light.brightness);
	}

	[Fact]
	public async Task GetAccessoryInfoTests()
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		var info = await _sut.GetInfoAsync(cts.Token);

		Assert.NotEmpty(info.displayName);
		Assert.NotEmpty(info.productName);
		Assert.NotEmpty(info.serialNumber);
	}
}
