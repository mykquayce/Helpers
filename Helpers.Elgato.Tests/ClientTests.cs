using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Net;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ClientTests : IClassFixture<Fixtures.ClientFixture>, IClassFixture<Fixtures.ConfigFixture>
{
	private readonly IClient _sut;
	private readonly IPAddress _keylightIPAddress, _lightstripIPAddress;

	public ClientTests(
		Fixtures.ClientFixture clientFixture,
		Fixtures.ConfigFixture configFixture)
	{
		_sut = clientFixture.Client;
		_keylightIPAddress = configFixture.KeylightIPAddress;
		_lightstripIPAddress = configFixture.LightstripIPAddress;
	}

	[Fact]
	public async Task GetAccessoryInfo()
	{
		var info = await _sut.GetAccessoryInfoAsync(_keylightIPAddress);

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

	[Fact]
	public async Task GetLight()
	{
		var light = await _sut.GetLightAsync(_keylightIPAddress);

		Assert.NotNull(light);
		Assert.InRange(light.brightness, 0, 100);
		Assert.InRange(light.on, 0, 1);
		Assert.NotNull(light.temperature);
		Assert.InRange(light.temperature!.Value, 140, 350);
	}

	[Theory]
	[InlineData(0, 23, 113, 72)]
	[InlineData(1, 23, 22, 85)]
	public async Task SetRgbLight(int on, int brightness, double hue, double saturation)
	{
		var before = new Models.Generated.LightObject(on, brightness, null, hue, saturation);

		await _sut.SetLightAsync(_lightstripIPAddress, before);

		var after = await _sut.GetLightAsync(_lightstripIPAddress);

		Assert.Equal(on, after.on);
		Assert.Equal(brightness, after.brightness);
		Assert.Equal(hue, after.hue);
		Assert.Equal(saturation, after.saturation);
	}

	[Theory]
	[InlineData(0, 25, 143)]
	[InlineData(1, 25, 143)]
	public async Task SetWhiteLight(int on, int brightness, int temperature)
	{
		var before = new Models.Generated.LightObject(on: on, brightness: brightness, temperature: temperature, hue: null, saturation: null);

		await _sut.SetLightAsync(_keylightIPAddress, before);

		var after = await _sut.GetLightAsync(_keylightIPAddress);

		Assert.Equal(on, after.on);
		Assert.Equal(brightness, after.brightness);
		Assert.Equal(temperature, after.temperature);
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

		await _sut.SetLightAsync(_lightstripIPAddress, before);

		var after = await _sut.GetLightAsync(_lightstripIPAddress);

		var comparer = TolerantEqualityComparer<double>.One;

		Assert.Equal(before.on, after.on);
		Assert.Equal(before.brightness, after.brightness);
		Assert.Equal(before.temperature, after.temperature);
		Assert.Equal(before.hue!.Value, after.hue!.Value, comparer);
		Assert.Equal(before.saturation!.Value, after.saturation!.Value, comparer);
	}
}

public class TolerantEqualityComparer<T> : IEqualityComparer<T>
	where T : IComparable<T>
{
	private readonly int _tolerance;

	private TolerantEqualityComparer(int tolerance)
	{
		_tolerance = tolerance;
	}

	public bool Equals(T? x, T? y) => x?.CompareTo(y) <= _tolerance;
	public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;

	public static TolerantEqualityComparer<T> Zero => new(0);
	public static TolerantEqualityComparer<T> One => new(1);
	public static TolerantEqualityComparer<T> Two => new(2);
}
