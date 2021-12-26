using System.Net;
using Xunit;

namespace Helpers.Elgato.Tests;

public class ElgatoServiceTests : IClassFixture<Fixtures.ConfigFixture>, IClassFixture<Fixtures.ElgatoServiceFixture>
{
	private readonly IPAddress _ipAddress;
	private readonly IElgatoService _sut;

	public ElgatoServiceTests(Fixtures.ConfigFixture configFixture, Fixtures.ElgatoServiceFixture serviceFixture)
	{
		_ipAddress = configFixture.Addresses.IPAddress;
		_sut = serviceFixture.Service;
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task SetPowerStateTests(bool on)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetPowerStateAsync(_ipAddress, on, cts.Token);
		var (actual, _, _) = await _sut.GetLightSettingsAsync(_ipAddress, cts.Token);
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
			await _sut.TogglePowerStateAsync(_ipAddress, cts.Token);
			var (on, _, _) = await _sut.GetLightSettingsAsync(_ipAddress, cts.Token);
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
		await _sut.SetBrightnessAsync(_ipAddress, brightness, cts.Token);
		var (_, actual, _) = await _sut.GetLightSettingsAsync(_ipAddress, cts.Token);
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
		await _sut.SetTemperatureAsync(_ipAddress, temperature, cts.Token);
		var (_, _, actual) = await _sut.GetLightSettingsAsync(_ipAddress, cts.Token);
		Assert.InRange(temperature - actual, -10, 10);
	}
}
