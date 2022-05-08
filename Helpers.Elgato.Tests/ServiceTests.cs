using System.Net;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>, IClassFixture<Fixtures.ConfigFixture>
{
	private readonly IService _sut;
	private readonly IPAddress _ip;

	public ServiceTests(
		Fixtures.ServiceFixture serviceFixture,
		Fixtures.ConfigFixture configFixture)
	{
		_sut = serviceFixture.Service;
		_ip = configFixture.KeylightIPAddress;
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task SetPowerStateTests(bool on)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetPowerStateAsync(_ip, on, cts.Token);
		(var actual, _) = await _sut.GetLightAsync(_ip, cts.Token);
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
			await _sut.TogglePowerStateAsync(_ip, cts.Token);
			(var on, _) = await _sut.GetLightAsync(_ip, cts.Token);
			states.Add(on);
		}

		Assert.Equal(2, states.Distinct().Count());
	}

	[Theory]
	[InlineData(0d)]
	[InlineData(0.1d)]
	[InlineData(0.5d)]
	[InlineData(1d)]
	public async Task SetBrightnessTests(float brightness)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetBrightnessAsync(_ip, brightness, cts.Token);
		(_, var actual) = await _sut.GetLightAsync(_ip, cts.Token);
		Assert.Equal(brightness, actual, precision: 2);
	}

	[Theory]
	[InlineData(2_900)]
	[InlineData(3_400)]
	[InlineData(5_600)]
	[InlineData(7_000)]
	public async Task SetKelvinsTests(short kelvins)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);
		await _sut.SetKelvinsAsync(_ip, kelvins, cts.Token);
		var light = await _sut.GetLightAsync(_ip, cts.Token);

		Assert.IsType<Models.WhiteLightObject>(light);

		var actual = light as Models.WhiteLightObject;

		Assert.NotNull(actual);
		Assert.InRange(actual!.Kelvins, kelvins - 10, kelvins + 10);
	}
}
