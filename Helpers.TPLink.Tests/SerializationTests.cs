using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class SerializationTests
	{

		/*{"system":{"get_sysinfo":{"sw_ver":"1.0.4 Build 191111 Rel.143903","hw_ver":"4.1","model":"HS110(UK)","deviceId":"8006D82222655E2A9134020CF3B18ABE1CFFD12E","oemId":"01F4EAFA6D419E341FB3A952C3E7D9A0","hwId":"469935032CA15745C0667D7BAD76EBC0","rssi":-53,"longitude_i":-21408,"latitude_i":535303,"alias":"amp","status":"new","mic_type":"IOT.SMARTPLUGSWITCH","feature":"TIM:ENE","mac":"B0:95:75:E4:F9:88","updating":0,"led_off":0,"relay_state":1,"on_time":296071,"active_mode":"none","icon_hash":"","dev_name":"Smart Wi-Fi Plug With Energy Monitoring","next_action":{"type":-1},"ntc_state":0,"err_code":0}},"emeter":{"get_realtime":{"voltage_mv":251227,"current_ma":31,"power_mw":1673,"total_wh":1015,"err_code":0}}}*/
		[Theory]
		[InlineData("{\"system\":{\"get_sysinfo\":{\"sw_ver\":\"1.0.4 Build 191111 Rel.143903\",\"hw_ver\":\"4.1\",\"model\":\"HS110(UK)\",\"deviceId\":\"8006D82222655E2A9134020CF3B18ABE1CFFD12E\",\"oemId\":\"01F4EAFA6D419E341FB3A952C3E7D9A0\",\"hwId\":\"469935032CA15745C0667D7BAD76EBC0\",\"rssi\":-53,\"longitude_i\":-21408,\"latitude_i\":535303,\"alias\":\"amp\",\"status\":\"new\",\"mic_type\":\"IOT.SMARTPLUGSWITCH\",\"feature\":\"TIM:ENE\",\"mac\":\"B0:95:75:E4:F9:88\",\"updating\":0,\"led_off\":0,\"relay_state\":1,\"on_time\":296071,\"active_mode\":\"none\",\"icon_hash\":\"\",\"dev_name\":\"Smart Wi-Fi Plug With Energy Monitoring\",\"next_action\":{\"type\":-1},\"ntc_state\":0,\"err_code\":0}},\"emeter\":{\"get_realtime\":{\"voltage_mv\":251227,\"current_ma\":31,\"power_mw\":1673,\"total_wh\":1015,\"err_code\":0}}}")]
		public void DeserializationTests(string json)
		{
			var actual = JsonSerializer.Deserialize<Models.ResponseDataObject>(json);

			Assert.NotNull(actual);
			Assert.NotNull(actual!.system);

			Assert.NotNull(actual.system!.get_sysinfo);
			Assert.NotNull(actual.system.get_sysinfo!.active_mode);
			Assert.NotNull(actual.system.get_sysinfo.alias);
			Assert.NotNull(actual.system.get_sysinfo.dev_name);
			Assert.NotNull(actual.system.get_sysinfo.deviceId);
			Assert.NotNull(actual.system.get_sysinfo.err_code);
			Assert.NotNull(actual.system.get_sysinfo.feature);
			Assert.NotNull(actual.system.get_sysinfo.hw_ver);
			Assert.NotNull(actual.system.get_sysinfo.hwId);
			Assert.NotNull(actual.system.get_sysinfo.icon_hash);
			Assert.NotNull(actual.system.get_sysinfo.latitude_i);
			Assert.NotNull(actual.system.get_sysinfo.led_off);
			Assert.NotNull(actual.system.get_sysinfo.longitude_i);
			Assert.NotNull(actual.system.get_sysinfo.mac);
			Assert.NotNull(actual.system.get_sysinfo.mic_type);
			Assert.NotNull(actual.system.get_sysinfo.model);
			Assert.NotNull(actual.system.get_sysinfo.next_action);
			Assert.NotNull(actual.system.get_sysinfo.next_action!.type);
			Assert.NotNull(actual.system.get_sysinfo.ntc_state);
			Assert.NotNull(actual.system.get_sysinfo.oemId);
			Assert.NotNull(actual.system.get_sysinfo.on_time);
			Assert.NotNull(actual.system.get_sysinfo.relay_state);
			Assert.NotNull(actual.system.get_sysinfo.rssi);
			Assert.NotNull(actual.system.get_sysinfo.status);
			Assert.NotNull(actual.system.get_sysinfo.sw_ver);
			Assert.NotNull(actual.system.get_sysinfo.updating);

			Assert.NotNull(actual.emeter);
			Assert.NotNull(actual.emeter!.get_realtime);
			Assert.NotNull(actual.emeter.get_realtime!.current_ma);
			Assert.NotNull(actual.emeter.get_realtime.err_code);
			Assert.NotNull(actual.emeter.get_realtime.power_mw);
			Assert.NotNull(actual.emeter.get_realtime.total_wh);
			Assert.NotNull(actual.emeter.get_realtime.voltage_mv);
		}

		[Theory]
		[InlineData("{\"system\":{\"get_sysinfo\":{}}}")]
		public void SerializationTest(string expected)
		{
			var o = new { system = new { get_sysinfo = new object(), }, };
			var actual = JsonSerializer.Serialize(o);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("{\"system\":{\"get_sysinfo\":{}},\"emeter\":{\"get_realtime\":{}}}")]
		public void SerializeGetRealtimeTest(string expected)
		{
			var o = new
			{
				system = new { get_sysinfo = new object(), },
				emeter = new { get_realtime = new object(), },
			};
			var actual = JsonSerializer.Serialize(o);
			Assert.Equal(expected, actual);
		}
	}
}
