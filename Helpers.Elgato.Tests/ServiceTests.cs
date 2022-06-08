using System.Drawing;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>, IClassFixture<Fixtures.ConfigFixture>
{
	private readonly IService _sut;
	private readonly IReadOnlyCollection<string> _aliases;

	public ServiceTests(
		Fixtures.ServiceFixture serviceFixture,
		Fixtures.ConfigFixture configFixture)
	{
		_sut = serviceFixture.Service;
		_aliases = configFixture.Aliases.AsReadOnly();
		Assert.NotEmpty(_aliases);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task SetPowerStateTests(bool on)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);

		foreach (var alias in _aliases)
		{
			await _sut.SetPowerStateAsync(alias, on, cts.Token);
			var lights = await _sut.GetLightStatusAsync(alias, cts.Token).ToListAsync(cts.Token);
			Assert.NotNull(lights);
			Assert.NotEmpty(lights);
			foreach ((var actual, _, _, _) in lights)
			{
				Assert.Equal(on, actual);
			}
		}
	}

	[Theory]
	[InlineData(8)]
	public async Task TogglePowerStateTests(int count)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);

		while (count-- > 0)
		{
			foreach (var alias in _aliases)
			{
				await _sut.TogglePowerStateAsync(alias, cts.Token);
				var lights = await _sut.GetLightStatusAsync(alias, cts.Token).ToListAsync(cts.Token);

				var after = lights.Select(tuple => tuple.on).Distinct().ToList();
				Assert.NotEmpty(after);
				Assert.Single(after);
			}
		}
	}

	[Theory]
	[InlineData(.2f)]
	[InlineData(.4f)]
	[InlineData(.6f)]
	public async Task SetBrightnessTests(float brightness)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);

		foreach (var alias in _aliases)
		{
			await _sut.SetBrightnessAsync(alias, brightness, cts.Token);

			var lights = _sut.GetLightStatusAsync(alias, cts.Token);

			await foreach ((_, var actual, _, _) in lights)
			{
				Assert.Equal(brightness, actual, precision: 2);
			}
		}
	}

	[Theory]
	[InlineData(255, 0, 0)]
	public async Task SetColorTests(int red, int green, int blue)
	{
		var color = Color.FromArgb(alpha: 255, red: red, green: green, blue: blue);
		using var cts = new CancellationTokenSource(millisecondsDelay: 20_000);

		foreach (var alias in _aliases)
		{
			var ok = await _sut.GetLightStatusAsync(alias, cts.Token).AllAsync(l => l.color.HasValue);

			if (!ok) continue;

			await _sut.SetColorAsync(alias, color, cts.Token);

			var actuals = _sut.GetRgbLightStatusAsync(alias, cts.Token);

			await foreach ((_, _, var actual) in actuals)
			{
				Assert.Equal(color, actual);
			}
		}
	}

	[Theory]
	[InlineData("keylight")]
	public async Task WhiteLightHasNullColor(string alias)
	{
		var lights = await _sut.GetLightStatusAsync(alias).ToListAsync();

		Assert.Single(lights);
		Assert.Null(lights[0].color);
	}

	[Theory]
	[InlineData("lightstrip")]
	public async Task RgbLightHasNullKelvins(string alias)
	{
		var lights = await _sut.GetLightStatusAsync(alias).ToListAsync();

		Assert.Single(lights);
		Assert.Null(lights[0].kelvins);
	}

	[Theory]
	[InlineData(2_900)]
	[InlineData(3_925)]
	[InlineData(4_950)]
	[InlineData(5_975)]
	[InlineData(7_000)]
	public async Task SetKelvinsTests(short kelvins)
	{
		var comparer = Comparers.TolerantEqualityComparer<short>.Ten;
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);

		foreach (var alias in _aliases)
		{
			var ok = await _sut.GetLightStatusAsync(alias, cts.Token)
				.AllAsync(l => l.kelvins.HasValue, cts.Token);

			if (!ok) continue;

			await _sut.SetKelvinsAsync(alias, kelvins, cts.Token);

			var actuals = _sut.GetLightStatusAsync(alias, cts.Token);

			await foreach ((_, _, _, var actual) in actuals)
			{
				Assert.NotNull(actual);
				Assert.Equal(kelvins, actual!.Value, comparer: comparer);
			}
		}
	}
}
