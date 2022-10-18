using System.Drawing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>, IClassFixture<Fixtures.ConfigFixture>
{
	private readonly IService _sut;
	private readonly IReadOnlyCollection<IPAddress> _ips;

	public ServiceTests(
		Fixtures.ServiceFixture serviceFixture,
		Fixtures.ConfigFixture configFixture)
	{
		_sut = serviceFixture.Service;
		_ips = configFixture.IPAddresses;
		Assert.NotEmpty(_ips);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task SetPowerStateTests(bool on)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);

		foreach (var ip in _ips)
		{
			await _sut.SetPowerStateAsync(ip, on, cts.Token);
			var lights = await _sut.GetLightStatusAsync(ip, cts.Token).ToListAsync(cts.Token);
			Assert.NotNull(lights);
			Assert.NotEmpty(lights);
			foreach ((var actual, _) in lights)
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
			foreach (var ip in _ips)
			{
				await _sut.TogglePowerStateAsync(ip, cts.Token);
				var lights = await _sut.GetLightStatusAsync(ip, cts.Token).ToListAsync(cts.Token);

				var after = lights.Select(tuple => tuple.On).Distinct().ToList();
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

		foreach (var ip in _ips)
		{
			await _sut.SetBrightnessAsync(ip, brightness, cts.Token);

			var lights = _sut.GetLightStatusAsync(ip, cts.Token);

			await foreach ((_, var actual) in lights)
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

		foreach (var ip in _ips)
		{
			var ok = await _sut.GetRgbLightStatusAsync(ip, cts.Token).AnyAsync(cts.Token);

			if (!ok) continue;

			await _sut.SetColorAsync(ip, color, cts.Token);

			var actuals = _sut.GetRgbLightStatusAsync(ip, cts.Token);

			await foreach ((_, _, var actual) in actuals)
			{
				Assert.Equal(color, actual);
			}
		}
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

		foreach (var ip in _ips)
		{
			var ok = await _sut.GetWhiteLightStatusAsync(ip, cts.Token)
				.AnyAsync(cts.Token);

			if (!ok) continue;

			await _sut.SetKelvinsAsync(ip, kelvins, cts.Token);

			var actuals = _sut.GetWhiteLightStatusAsync(ip, cts.Token);

			await foreach ((_, _, var actual) in actuals)
			{
				Assert.Equal(kelvins, actual, comparer: comparer);
			}
		}
	}

	[Theory]
	[InlineData(3_000)]
	public async Task GetLightStatusTests(int timeout)
	{
		foreach (var ip in _ips)
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: timeout);
			await foreach (var light in _sut.GetLightStatusAsync(ip, cts.Token))
			{
				Assert.True(light is Models.Lights.RgbLightModel ^ light is Models.Lights.WhiteLightModel);
			}
		}
	}

	[Fact]
	public async Task ReturnTypesAreSerializable()
	{
		foreach (var ip in _ips)
		{
			await test(_sut.GetLightStatusAsync(ip));
			await test(_sut.GetRgbLightStatusAsync(ip));
			await test(_sut.GetWhiteLightStatusAsync(ip));
		}

		static async Task test<T>(IAsyncEnumerable<T> values)
		{
			await foreach (var value in values)
			{
				var json = JsonSerializer.Serialize(value);
				Assert.NotNull(json);
				Assert.NotEmpty(json);
				Assert.NotEqual("{}", json);
			}
		}
	}
}
