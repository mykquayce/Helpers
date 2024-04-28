using Helpers.Elgato.Models;
using System.Text.Json;
using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(NonParallelCollectionDefinitionClass))]
public class Serialization
{
	[Theory]
	[InlineData("""{"numberOfLights":1,"lights":[{"on":0,"brightness":23,"temperature":331}]}""", 0, 23, 331)]
	public void Message(string json, byte on, byte brightness, int? temperature)
	{
		var message = JsonSerializer.Deserialize<Message<WhiteLight>>(json);

		Assert.NotEqual(default, message);
		Assert.Equal(1, message!.numberOfLights);
		Assert.NotNull(message.lights);
		Assert.NotEmpty(message.lights);
		Assert.Single(message.lights);

		var light = message.lights.First();

		Assert.Equal(on, light.on);
		Assert.Equal(brightness, light.brightness);
		Assert.Equal(temperature, light.temperature);
	}

	[Theory]
	[InlineData("""{"productName":"Elgato Key Light","hardwareBoardType":53,"firmwareBuildNumber":200,"firmwareVersion":"1.0.3","serialNumber":"BW33J1A02740","displayName":"Elgato Key Light 227A","features":["lights"]}""")]
	public void AccessoryInfo(string json)
	{
		var info = JsonSerializer.Deserialize<Info>(json);

		Assert.NotEqual(default, info);
		f(info!.productName);
		Assert.InRange(info.hardwareBoardType, 1, int.MaxValue);
		Assert.InRange(info.firmwareBuildNumber, 1, int.MaxValue);
		Assert.NotNull(info.firmwareVersion);
		Assert.InRange(info.firmwareVersion.Major, 1, int.MaxValue);
		f(info.serialNumber);
		f(info.displayName);
		Assert.NotNull(info.features);
		Assert.NotEmpty(info.features);
		Assert.All(info.features, f);

		static void f(string s) { Assert.NotNull(s); Assert.NotEmpty(s); Assert.DoesNotMatch(@"^\s", s); };
	}
}
