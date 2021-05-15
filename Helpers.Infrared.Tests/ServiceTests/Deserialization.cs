using System.Text.Json;
using Xunit;

namespace Helpers.Infrared.Tests.ServiceTests
{

	public class Deserialization
	{
		[Theory]
		[InlineData(@"{""amp"":""iTach059CAD""}", "amp")]
		public void Devices(string json, params string[] keys)
		{
			var actual = JsonSerializer.Deserialize<Services.Concrete.InfraredService.Devices>(json);

			Assert.NotNull(actual);
			Assert.NotEmpty(actual);
			Assert.Equal(keys.Length, actual!.Count);

			foreach (var key in keys)
			{
				Assert.Contains(key, actual.Keys);
				var value = actual[key];
				Assert.NotNull(value);
				Assert.NotEmpty(value);
			}
		}

		[Theory]
		[InlineData(@"{""ToggleMute"":""sendir,1:1,3,40064,3,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r""}", Models.SignalTypes.ToggleMute)]
		[InlineData(@"{
				""ToggleMute"":""sendir,1:1,3,40064,3,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r"",
				""TogglePower"":""sendir,1:1,3,40192,3,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r"",
				""VolumeUp"":""sendir,1:1,3,40192,3,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r"",
				""VolumeDown"":""sendir,1:1,3,40192,3,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r""
			}", Models.SignalTypes.ToggleMute, Models.SignalTypes.TogglePower, Models.SignalTypes.VolumeDown, Models.SignalTypes.VolumeDown)]
		public void Signals(string json, params Models.SignalTypes[] keys)
		{
			var actual = JsonSerializer.Deserialize<Services.Concrete.InfraredService.Signals>(json);

			Assert.NotNull(actual);
			Assert.NotEmpty(actual);
			Assert.Equal(keys.Length, actual!.Count);

			foreach (var key in keys)
			{
				Assert.Contains(key, actual.Keys);
				var value = actual[key];
				Assert.NotNull(value);
				Assert.NotEmpty(value);
			}
		}
	}
}
