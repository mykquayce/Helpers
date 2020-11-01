using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class DiscoverTests
	{
		private readonly ITPLinkUdpClient _sut;

		public DiscoverTests()
		{
			var encryptionService = new Concrete.EncryptionService(0xAB);
			var config = new Concrete.TPLinkUdpClient.Config { MillisecondsTimeout = 5_000, Port = 9_999, };

			_sut = new Concrete.TPLinkUdpClient(Options.Create(config), encryptionService);
		}

		[Fact]
		public async Task DiscoverTest()
		{
			// Act
			var sysInfo = await _sut.DiscoverAsync();

			// Assert
			Assert.NotNull(sysInfo);

			Assert.NotNull(sysInfo!.active_mode);
			Assert.NotNull(sysInfo.alias);
			Assert.NotNull(sysInfo.dev_name);
			Assert.NotNull(sysInfo.deviceId);
			Assert.NotNull(sysInfo.err_code);
			Assert.NotNull(sysInfo.feature);
			Assert.NotNull(sysInfo.hw_ver);
			Assert.NotNull(sysInfo.hwId);
			Assert.NotNull(sysInfo.icon_hash);
			Assert.NotNull(sysInfo.latitude_i);
			Assert.NotNull(sysInfo.led_off);
			Assert.NotNull(sysInfo.longitude_i);
			Assert.NotNull(sysInfo.mac);
			Assert.NotNull(sysInfo.mic_type);
			Assert.NotNull(sysInfo.model);
			Assert.NotNull(sysInfo.next_action);
			Assert.NotNull(sysInfo.next_action!.type);
			Assert.NotNull(sysInfo.ntc_state);
			Assert.NotNull(sysInfo.oemId);
			Assert.NotNull(sysInfo.on_time);
			Assert.NotNull(sysInfo.relay_state);
			Assert.NotNull(sysInfo.rssi);
			Assert.NotNull(sysInfo.status);
			Assert.NotNull(sysInfo.sw_ver);
			Assert.NotNull(sysInfo.updating);
		}

		[Fact]
		public async Task GetRealtimeDataTest()
		{
			var realtime = await _sut.GetRealtimeAsync();

			Assert.NotNull(realtime);
			Assert.NotNull(realtime!.current_ma);
			Assert.NotNull(realtime.err_code);
			Assert.NotNull(realtime.power_mw);
			Assert.NotNull(realtime.total_wh);
			Assert.NotNull(realtime.voltage_mv);
		}
	}
}
