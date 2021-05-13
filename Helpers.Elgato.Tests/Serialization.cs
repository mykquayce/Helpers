using System.Text.Json;
using Xunit;

namespace Helpers.Elgato.Tests
{
	public class Serialization
	{
		[Theory]
		[InlineData(@"{""numberOfLights"":1,""lights"":[{""on"":0,""brightness"":23,""temperature"":331}]}")]
		public void Message(string json)
		{
			var message = JsonSerializer.Deserialize<Models.MessageObject>(json);

			Assert.NotNull(message);
			Assert.NotEqual(0, message!.numberOfLights);
			Assert.NotNull(message.lights);
			Assert.NotEmpty(message.lights);

			foreach (var light in message.lights)
			{
				Assert.NotNull(light);
				Assert.InRange(light.on, 0, 1);
				Assert.InRange(light.brightness, 0, 100);
				Assert.InRange(light.temperature, 140, 350);
			}
		}
		[Theory]
		[InlineData(@"{""productName"":""Elgato Key Light"",""hardwareBoardType"":53,""firmwareBuildNumber"":200,""firmwareVersion"":""1.0.3"",""serialNumber"":""BW33J1A02740"",""displayName"":""Elgato Key Light 227A"",""features"":[""lights""]}")]
		public void AccessoryInfo(string json)
		{
			var info = JsonSerializer.Deserialize<Models.AccessoryInfoObject>(json);

			Assert.NotNull(info);
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
}
