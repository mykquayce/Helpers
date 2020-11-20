using System.Text.Json;
using Xunit;

namespace Helpers.TPLink.Models.Tests
{
	public class GetDeviceListTests
	{
		[Theory]
		[InlineData(@"{
    ""error_code"": 0,
    ""result"": {
        ""deviceList"": [
            {
                ""deviceType"": ""IOT.SMARTPLUGSWITCH"",
                ""role"": 0,
                ""fwVer"": ""1.0.4 Build 191111 Rel.143903"",
                ""appServerUrl"": ""https://eu-wap.tplinkcloud.com"",
                ""deviceRegion"": ""eu-west-1"",
                ""deviceId"": ""8F82799AD064C64873B8F39DBE82BCBE5176EA0C"",
                ""deviceName"": ""Smart Wi-Fi Plug With Energy Monitoring"",
                ""deviceHwVer"": ""4.1"",
                ""alias"": ""amp"",
                ""deviceMac"": ""2915DCB9D9B6"",
                ""oemId"": ""3AB3BF7140BBEFD7622CC9A6C54693CF"",
                ""deviceModel"": ""HS110(UK)"",
                ""hwId"": ""BA3833EE69ED1CDAA22E00CAB2DB6355"",
                ""fwId"": ""00000000000000000000000000000000"",
                ""isSameRegion"": true,
                ""status"": 1
            }
        ]
    }
}")]

		public void DeserializeResponse_HasValues(string json)
		{
			var actual = JsonSerializer.Deserialize<GetDeviceListResponseObject>(json);

			Assert.NotNull(actual);
			Assert.Equal(Enums.ErrorCode.None, actual!.error_code);
			Assert.NotNull(actual.result);
			Assert.NotNull(actual.result!.deviceList);
			Assert.NotEmpty(actual.result!.deviceList);
			Assert.Single(actual.result!.deviceList);

			var device = actual.result!.deviceList![0];

			Assert.NotNull(device);
			Assert.Equal("IOT.SMARTPLUGSWITCH", device.deviceType);
			Assert.Equal(0, device.role);
			Assert.Equal("1.0.4 Build 191111 Rel.143903", device.fwVer);
			Assert.Equal("https://eu-wap.tplinkcloud.com", device.appServerUrl);
			Assert.Equal("eu-west-1", device.deviceRegion);
			Assert.Equal("8F82799AD064C64873B8F39DBE82BCBE5176EA0C", device.deviceId);
			Assert.Equal("Smart Wi-Fi Plug With Energy Monitoring", device.deviceName);
			Assert.Equal("4.1", device.deviceHwVer);
			Assert.Equal("amp", device.alias);
			Assert.Equal("2915DCB9D9B6", device.deviceMac);
			Assert.Equal("3AB3BF7140BBEFD7622CC9A6C54693CF", device.oemId);
			Assert.Equal("HS110(UK)", device.deviceModel);
			Assert.Equal("BA3833EE69ED1CDAA22E00CAB2DB6355", device.hwId);
			Assert.Equal("00000000000000000000000000000000", device.fwId);
			Assert.True(device.isSameRegion);
			Assert.Equal(1, device.status);
		}
	}
}
