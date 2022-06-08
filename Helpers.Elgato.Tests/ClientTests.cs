using Helpers.Elgato.Tests.Comparers;
using System.Drawing;
using System.Net;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ClientTests : IClassFixture<Fixtures.ClientFixture>, IClassFixture<Fixtures.ConfigFixture>
{
	private readonly IClient _sut;
	private readonly IReadOnlyCollection<IPAddress> _ipAddresses;

	public ClientTests(
		Fixtures.ClientFixture clientFixture,
		Fixtures.ConfigFixture configFixture)
	{
		_sut = clientFixture.Client;
		_ipAddresses = configFixture.IPAddresses.AsReadOnly();
	}

	[Fact]
	public async Task GetWhiteLightTests()
	{
		foreach (var ipAddress in _ipAddresses)
		{
			var lights = _sut.GetLightsAsync(ipAddress);

			await foreach (var light in lights)
			{
				Assert.True(light.IsRgb || light.IsWhite);
			}
		}
	}

	[Fact]
	public async Task GetAccessoryInfo()
	{
		foreach (var ipAddress in _ipAddresses)
		{
			var info = await _sut.GetAccessoryInfoAsync(ipAddress);

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
	}

	[Fact]
	public async Task GetLight()
	{
		foreach (var ipAddress in _ipAddresses)
		{
			var lights = await _sut.GetLightsAsync(ipAddress).ToListAsync();

			Assert.NotNull(lights);
			Assert.NotEmpty(lights);

			foreach (var light in lights)
			{
				Assert.NotNull(light);
				Assert.InRange(light.brightness, 0, 100);
				Assert.InRange(light.on, 0, 1);

				if (light.IsWhite)
				{
					Assert.NotNull(light.temperature);
					Assert.InRange(light.temperature!.Value, 140, 350);
				}

				if (light.IsRgb)
				{
					Assert.NotNull(light.hue);
					Assert.InRange(light.hue!.Value, 0, 360);
					Assert.NotNull(light.saturation);
					Assert.InRange(light.saturation!.Value, 0, 100);
				}
			}
		}
	}

	[Theory]
	[InlineData(0, 23, 113, 72)]
	[InlineData(1, 23, 22, 85)]
	public async Task SetRgbLight(int on, int brightness, double hue, double saturation)
	{
		foreach (var ipAddress in _ipAddresses)
		{
			if (await _sut.GetLightsAsync(ipAddress).AnyAsync(l => l.IsWhite))
			{
				continue;
			}

			var before = new Models.Generated.LightObject(on, brightness, null, hue, saturation);

			await _sut.SetLightAsync(ipAddress, new[] { before, });

			var lights = await _sut.GetLightsAsync(ipAddress).ToListAsync();

			Assert.NotNull(lights);
			Assert.NotEmpty(lights);

			foreach (var after in lights)
			{
				Assert.Equal(on, after.on);
				Assert.Equal(brightness, after.brightness);
				Assert.Equal(hue, after.hue);
				Assert.Equal(saturation, after.saturation);
			}
		}
	}

	[Theory]
	[InlineData(0, 25, 143)]
	[InlineData(1, 25, 143)]
	public async Task SetWhiteLight(int on, int brightness, int temperature)
	{
		foreach (var ipAddress in _ipAddresses)
		{
			if (await _sut.GetLightsAsync(ipAddress).AnyAsync(l => l.IsRgb))
			{
				continue;
			}

			var before = new Models.Generated.LightObject(on: on, brightness: brightness, temperature: temperature, hue: null, saturation: null);

			await _sut.SetLightAsync(ipAddress, new[] { before, });

			var lights = await _sut.GetLightsAsync(ipAddress).ToListAsync();

			Assert.NotNull(lights);
			Assert.NotEmpty(lights);

			foreach (var after in lights)
			{
				Assert.Equal(on, after.on);
				Assert.Equal(brightness, after.brightness);
				Assert.Equal(temperature, after.temperature);
			}
		}
	}

	[Theory]
	[InlineData(KnownColor.ActiveCaption)]
	[InlineData(KnownColor.Azure)]
	[InlineData(KnownColor.ControlDark)]
	[InlineData(KnownColor.ControlDarkDark)]
	[InlineData(KnownColor.CornflowerBlue)]
	[InlineData(KnownColor.DarkOrange)]
	[InlineData(KnownColor.DarkSeaGreen)]
	[InlineData(KnownColor.DeepPink)]
	[InlineData(KnownColor.Gold)]
	[InlineData(KnownColor.Gray)]
	[InlineData(KnownColor.Lavender)]
	[InlineData(KnownColor.LightCoral)]
	[InlineData(KnownColor.LightGray)]
	[InlineData(KnownColor.LightGreen)]
	[InlineData(KnownColor.LightSkyBlue)]
	[InlineData(KnownColor.LimeGreen)]
	[InlineData(KnownColor.Salmon)]
	[InlineData(KnownColor.Transparent)]
	[InlineData(KnownColor.Yellow)]
	public async Task SetRgbLightToColor(KnownColor knownColor)
	{
		var color = Color.FromKnownColor(knownColor);
		var hsbColor = color.GetHsbColor();

		var (hue, saturation, brightness) = hsbColor;

		var before = new Models.Generated.LightObject(
			on: 1,
			brightness: (int)Math.Round(brightness * 100d),
			temperature: null,
			hue: hue,
			saturation: saturation * 100d);

		foreach (var ipAddress in _ipAddresses)
		{
			if (await _sut.GetLightsAsync(ipAddress).AnyAsync(l => l.IsWhite))
			{
				continue;
			}

			await _sut.SetLightAsync(ipAddress, new[] { before, });

			var lights = await _sut.GetLightsAsync(ipAddress).ToListAsync();

			Assert.NotNull(lights);
			Assert.NotEmpty(lights);

			var comparer = TolerantEqualityComparer<double>.One;

			foreach (var after in lights)
			{
				Assert.Equal(before.on, after.on);
				Assert.Equal(before.brightness, after.brightness);
				Assert.Equal(before.temperature, after.temperature);
				Assert.Equal(before.hue!.Value, after.hue!.Value, comparer);
				Assert.Equal(before.saturation!.Value, after.saturation!.Value, comparer);
			}
		}
	}
}
