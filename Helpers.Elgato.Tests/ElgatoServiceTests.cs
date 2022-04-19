using System.Net;
using Xunit;

namespace Helpers.Elgato.Tests;

public class ElgatoServiceTests : IClassFixture<Fixtures.ElgatoServiceFixture>
{
	private readonly IElgatoService _sut;

	public ElgatoServiceTests(Fixtures.ElgatoServiceFixture serviceFixture)
	{
		_sut = serviceFixture.Service;
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task SetPowerStateTests(bool on)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetPowerStateAsync( on, cts.Token);
		var (actual, _, _) = await _sut.GetLightSettingsAsync(cts.Token);
		Assert.Equal(on, actual);
	}

	[Theory]
	[InlineData(2)]
	public async Task TogglePowerStateTests(int count)
	{
		var states = new List<bool>(capacity: count);
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);

		while (count-- > 0
			&& !cts.IsCancellationRequested)
		{
			await _sut.TogglePowerStateAsync(cts.Token);
			var (on, _, _) = await _sut.GetLightSettingsAsync(cts.Token);
			states.Add(on);
		}

		Assert.Equal(2, states.Distinct().Count());
	}

	[Theory]
	[InlineData(0d)]
	[InlineData(0.1d)]
	[InlineData(0.5d)]
	[InlineData(1d)]
	public async Task SetBrightnessTests(double brightness)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetBrightnessAsync(brightness, cts.Token);
		var (_, actual, _) = await _sut.GetLightSettingsAsync(cts.Token);
		Assert.Equal(brightness, actual, precision: 2);
	}

	[Theory]
	[InlineData(2_900)]
	[InlineData(3_400)]
	[InlineData(5_600)]
	[InlineData(7_000)]
	public async Task SetTemperatureTests(short temperature)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetTemperatureAsync(temperature, cts.Token);
		var (_, _, actual) = await _sut.GetLightSettingsAsync(cts.Token);
		Assert.InRange(temperature - actual, -10, 10);
	}
}
