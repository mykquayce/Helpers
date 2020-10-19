using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class TPLinkClientTests
	{
		private readonly Helpers.TPLink.ITPLinkClient _sut;
		private readonly string _userName, _password, _token, _deviceId;

		public TPLinkClientTests()
		{
			var config = new ConfigurationBuilder()
			.AddUserSecrets("8391cb70-d94f-4863-b7e4-5659af167bc6")
			.Build();

			_userName = config["TPLink:UserName"] ?? throw new ArgumentNullException("TPLink:UserName");
			_password = config["TPLink:Password"] ?? throw new ArgumentNullException("TPLink:Password");
			_token = config["TPLink:Token"] ?? throw new ArgumentNullException("TPLink:Token");
			_deviceId = config["TPLink:DeviceId"] ?? throw new ArgumentNullException("TPLink:DeviceId");

			_sut = new Helpers.TPLink.Concrete.TPLinkClient();
		}

		[Fact]
		public async Task LoginTest()
		{
			// Act
			var token = await _sut.LoginAsync(_userName, _password);

			// Assert
			Assert.NotNull(token);
			Assert.NotEmpty(token);
			Assert.Matches("^6bdd39aa-[0-9A-Za-z]{23}$", token);
		}

		[Fact]
		public async Task GetDevices()
		{
			var devices = await _sut.GetDevicesAsync(_token).ToListAsync();

			Assert.NotNull(devices);
			Assert.NotEmpty(devices);
			Assert.Single(devices);

			var device = devices![0];

			Assert.NotNull(device);
			Assert.Equal("IOT.SMARTPLUGSWITCH", device.deviceType);
			Assert.Equal(0, device.role);
			Assert.Equal("1.0.4 Build 191111 Rel.143903", device.fwVer);
			Assert.Equal("https://eu-wap.tplinkcloud.com", device.appServerUrl);
			Assert.Equal("eu-west-1", device.deviceRegion);
			Assert.Matches("^[0-9A-Z]{40}$", device.deviceId);
			Assert.Equal("Smart Wi-Fi Plug With Energy Monitoring", device.deviceName);
			Assert.Equal("4.1", device.deviceHwVer);
			Assert.Equal("amp", device.alias);
			Assert.Matches("[0-9A-F]{12}", device.deviceMac);
			Assert.Matches("^[0-9A-Z]{32}$", device.oemId);
			Assert.Equal("HS110(UK)", device.deviceModel);
			Assert.Matches("^[0-9A-Z]{32}$", device.hwId);
			Assert.Equal("00000000000000000000000000000000", device.fwId);
			Assert.True(device.isSameRegion);
			Assert.Equal(1, device.status);
		}

		[Fact]
		public async Task GetRealtimeData()
		{
			var actual = await _sut.GetRealtimeDataAsync(_token, _deviceId);

			Assert.NotNull(actual);
			Assert.NotNull(actual.voltage_mv);
			Assert.InRange(actual.voltage_mv!.Value, 220_000, 260_000);
			Assert.NotNull(actual.current_ma);
			Assert.InRange(actual.current_ma!.Value, 10, 200);
			Assert.NotNull(actual.power_mw);
			Assert.InRange(actual.power_mw!.Value, 1_000, 20_000);
			Assert.NotNull(actual.total_wh);
			Assert.InRange(actual.total_wh!.Value, 1, int.MaxValue);
			Assert.Equal(0, actual.err_code);
		}
	}
}
