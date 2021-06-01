using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class SerializationTests
	{
		[Theory]
		[InlineData(@"{""system"":{""get_sysinfo"":{""sw_ver"":""1.0.16 Build 210205 Rel.163735"",""hw_ver"":""1.0"",""model"":""KP115(UK)"",""deviceId"":""800695CFD4D150EEAA174159B79A2D581E27C285"",""oemId"":""C7A36E0C2D4BAB44DED6EF0870AC707F"",""hwId"":""39E8408ED974DD69D8A77D9F8781637E"",""rssi"":-57,""latitude_i"":535303,""longitude_i"":-21408,""alias"":""amp"",""status"":""new"",""obd_src"":""tplink"",""mic_type"":""IOT.SMARTPLUGSWITCH"",""feature"":""TIM:ENE"",""mac"":""00:31:92:E1:A4:74"",""updating"":0,""led_off"":0,""relay_state"":1,""on_time"":176019,""icon_hash"":"""",""dev_name"":""Smart Wi-Fi Plug Mini"",""active_mode"":""none"",""next_action"":{""type"":-1},""ntc_state"":0,""err_code"":0}}}")]
		public void SystemInfo(string json)
		{
			var response = JsonSerializer.Deserialize<Models.Generated.ResponseObject>(json);

			Assert.NotNull(response);
			Assert.NotNull(response!.system);
			Assert.NotNull(response.system!.get_sysinfo);

			var systemInfo = response.system.get_sysinfo;

			Assert.NotNull(systemInfo!.alias);
			Assert.NotNull(systemInfo.mac);
		}
	}
}
