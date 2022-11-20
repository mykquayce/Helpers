using System;
using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Models.Tests
{
	public class ResponseObjectTests
	{
		[Theory]
		[InlineData(
			@"{""system"":{""get_sysinfo"":{""sw_ver"":""1.0.16 Build 210205 Rel.163735"",""hw_ver"":""1.0"",""model"":""KP115(UK)"",""deviceId"":""800695CFD4D150EEAA174159B79A2D581E27C285"",""oemId"":""C7A36E0C2D4BAB44DED6EF0870AC707F"",""hwId"":""39E8408ED974DD69D8A77D9F8781637E"",""rssi"":-57,""latitude_i"":535303,""longitude_i"":-21408,""alias"":""amp"",""status"":""new"",""obd_src"":""tplink"",""mic_type"":""IOT.SMARTPLUGSWITCH"",""feature"":""TIM:ENE"",""mac"":""00:31:92:E1:A4:74"",""updating"":0,""led_off"":0,""relay_state"":1,""on_time"":18647,""icon_hash"":"""",""dev_name"":""Smart Wi-Fi Plug Mini"",""active_mode"":""none"",""next_action"":{""type"":-1},""ntc_state"":0,""err_code"":0}},""emeter"":{""get_realtime"":{""current_ma"":18,""voltage_mv"":244162,""power_mw"":0,""total_wh"":33,""err_code"":0}}}",
			"amp", "003192e1a474", 18, 244162, 0)]
		public void SysInfo(string json, string expectedAlias, string expectedPhysicalAddress, int? expectedMilliamps, int? expectedMilliVolts, int? expectedMilliwatts)
		{
			var o = JsonSerializer.Deserialize<ResponseObject>(json);
			Assert.NotEqual(default, o);
			Assert.NotEqual(default, o.system);
			Assert.NotEqual(default, o.system.get_sysinfo);
			Assert.Equal(expectedAlias, o.system.get_sysinfo.alias);
			Assert.NotNull(o.system.get_sysinfo.mac);
			Assert.Equal(expectedPhysicalAddress, o.system.get_sysinfo.mac.ToString(), StringComparer.InvariantCultureIgnoreCase);
			Assert.Equal(expectedMilliamps, o.emeter.get_realtime.current_ma);
			Assert.Equal(expectedMilliVolts, o.emeter.get_realtime.voltage_mv);
			Assert.Equal(expectedMilliwatts, o.emeter.get_realtime.power_mw);
		}
	}
}
