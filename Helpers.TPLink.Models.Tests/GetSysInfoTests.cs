using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Models.Tests
{
	public class GetSysInfoTests
	{
		[Theory]
		[InlineData(@"{
  ""method"": ""passthrough"",
  ""params"": {
    ""deviceId"": ""4E08D8D3A3EF67E7AE8DB654CB0AC459BE24B276"",
    ""requestData"": ""{\""system\"":{\""get_sysinfo\"":null},\""emeter\"":{\""get_realtime\"":null}}""
  }
}")]
		public void CreateGetSysInfoRequest(string expected)
		{
			var requestData = new Dictionary<string, IDictionary<string, object?>>
			{
				["system"] = new Dictionary<string, object?> { ["get_sysinfo"] = default, },
				["emeter"] = new Dictionary<string, object?> { ["get_realtime"] = default, },
			};

			var requestDataJson = Serialize(requestData);

			var before = new GetSysInfoRequestObject
			{
				@params = new Dictionary<string, string>()
				{
					["deviceId"] = "4E08D8D3A3EF67E7AE8DB654CB0AC459BE24B276",
					["requestData"] = requestDataJson,
				}
			};

			var actual = Serialize(before, writeIndented: true);

			Assert.Equal(expected, actual);
		}

		private static string Serialize(object value, bool writeIndented = false)
		{
			var options = new JsonSerializerOptions { WriteIndented = writeIndented, };
			return JsonSerializer.Serialize(value, options).Replace("\\u0022", "\\\"");
		}

		[Theory]
		[InlineData(@"{
    ""error_code"": 0,
    ""result"": {
        ""responseData"": ""{\""system\"":{\""get_sysinfo\"":{\""sw_ver\"":\""1.0.4 Build 191111 Rel.143903\"",\""hw_ver\"":\""4.1\"",\""model\"":\""HS110(UK)\"",\""deviceId\"":\""12FEB944F3CA74A72D52E1F3CF0DA7F477DCFD04\"",\""oemId\"":\""C469C63594B41C7FCD3674D71270A1A2\"",\""hwId\"":\""BFB68EDE091D40D0EFCDD935C97AC507\"",\""rssi\"":-63,\""longitude_i\"":12345,\""latitude_i\"":67890,\""alias\"":\""amp\"",\""status\"":\""new\"",\""mic_type\"":\""IOT.SMARTPLUGSWITCH\"",\""feature\"":\""TIM:ENE\"",\""mac\"":\""FE:B8:67:CF:54:8F\"",\""updating\"":0,\""led_off\"":0,\""relay_state\"":1,\""on_time\"":80153,\""active_mode\"":\""none\"",\""icon_hash\"":\""\"",\""dev_name\"":\""Smart Wi-Fi Plug With Energy Monitoring\"",\""next_action\"":{\""type\"":-1},\""ntc_state\"":0,\""err_code\"":0}},\""emeter\"":{\""get_realtime\"":{\""voltage_mv\"":243781,\""current_ma\"":84,\""power_mw\"":9732,\""total_wh\"":107,\""err_code\"":0}}}""
    }
}")]
		public void DeserializeGetSysInfoResponse(string json)
		{
			var response = JsonSerializer.Deserialize<GetSysInfoResponseObject>(json);

			Assert.NotNull(response);
			Assert.NotNull(response!.result);
			Assert.NotNull(response.result!.responseData);

			var responseData = JsonSerializer.Deserialize<ResponseDataObject>(response.result.responseData!);

			Assert.NotNull(responseData);

			Assert.NotNull(responseData!.emeter);
			Assert.NotNull(responseData.emeter!.get_realtime);
			Assert.Equal(243781, responseData.emeter.get_realtime!.voltage_mv);
			Assert.Equal(84, responseData.emeter.get_realtime!.current_ma);
			Assert.Equal(9732, responseData.emeter.get_realtime!.power_mw);
			Assert.Equal(107, responseData.emeter.get_realtime!.total_wh);
			Assert.Equal(0, responseData.emeter.get_realtime!.err_code);

			Assert.NotNull(responseData.system);
			Assert.NotNull(responseData.system!.get_sysinfo);
			Assert.Equal("1.0.4 Build 191111 Rel.143903", responseData.system.get_sysinfo!.sw_ver);
			Assert.Equal("4.1", responseData.system.get_sysinfo.hw_ver);
			Assert.Equal("HS110(UK)", responseData.system.get_sysinfo.model);
			Assert.Equal("12FEB944F3CA74A72D52E1F3CF0DA7F477DCFD04", responseData.system.get_sysinfo.deviceId);
			Assert.Equal("C469C63594B41C7FCD3674D71270A1A2", responseData.system.get_sysinfo.oemId);
			Assert.Equal("BFB68EDE091D40D0EFCDD935C97AC507", responseData.system.get_sysinfo.hwId);
			Assert.Equal(-63, responseData.system.get_sysinfo.rssi);
			Assert.Equal(12345, responseData.system.get_sysinfo.longitude_i);
			Assert.Equal(67890, responseData.system.get_sysinfo.latitude_i);
			Assert.Equal("amp", responseData.system.get_sysinfo.alias);
			Assert.Equal("new", responseData.system.get_sysinfo.status);
			Assert.Equal("IOT.SMARTPLUGSWITCH", responseData.system.get_sysinfo.mic_type);
			Assert.Equal("TIM:ENE", responseData.system.get_sysinfo.feature);
			Assert.Equal("FE:B8:67:CF:54:8F", responseData.system.get_sysinfo.mac);
			Assert.Equal(0, responseData.system.get_sysinfo.updating);
			Assert.Equal(0, responseData.system.get_sysinfo.led_off);
			Assert.Equal(1, responseData.system.get_sysinfo.relay_state);
			Assert.Equal(80153, responseData.system.get_sysinfo.on_time);
			Assert.Equal("none", responseData.system.get_sysinfo.active_mode);
			Assert.Empty(responseData.system.get_sysinfo.icon_hash);
			Assert.Equal("Smart Wi-Fi Plug With Energy Monitoring", responseData.system.get_sysinfo.dev_name);
			Assert.NotNull(responseData.system.get_sysinfo.next_action);
			Assert.Equal(-1, responseData.system.get_sysinfo.next_action!.type);
			Assert.Equal(0, responseData.system.get_sysinfo.ntc_state);
			Assert.Equal(0, responseData.system.get_sysinfo.err_code);
		}
	}
}
