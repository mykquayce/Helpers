using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class TPLinkUdpServiceTests : IClassFixture<Fixtures.TPLinkUdpClientFixture>
	{
		private readonly ITPLinkUdpClient _sut;

		public TPLinkUdpServiceTests(Fixtures.TPLinkUdpClientFixture fixture)
		{
			_sut = fixture.TPLinkUdpClient;
		}

		[Theory]
		[InlineData(
			30,
			0xd0, 0xf2, 0x81, 0xf8, 0x8b, 0xff, 0x9a, 0xf7, 0xd5, 0xef, 0x94, 0xb6, 0xd1, 0xb4, 0xc0, 0x9f, 0xec, 0x95, 0xe6, 0x8f, 0xe1, 0x87, 0xe8, 0xca, 0xf0, 0x8b, 0xf6, 0x8b, 0xf6)]
		public async Task SendAndReceiveAsyncTest(int count, params int[] messageInts)
		{
			var messageBytes = messageInts.Select(i => (byte)i).ToArray();

			while (count-- > 0)
			{
				var response = await _sut.SendAndReceiveBytesAsync(messageBytes);

				Assert.InRange(response.Length, 100, 1_000);
			}
		}

		[Fact]
		public async Task SendAndReceiveObjectTest()
		{
			var requestObject = new { system = new { get_sysinfo = new object(), }, };
			var responseObject = await _sut.SendAndReceiveObjectAsync<Models.ResponseDataObject>(requestObject);

			// Assert
			Assert.NotNull(responseObject);
			Assert.NotNull(responseObject!.system);
			Assert.NotNull(responseObject.system!.get_sysinfo);
			Assert.NotNull(responseObject.system.get_sysinfo!.deviceId);
			Assert.NotEmpty(responseObject.system.get_sysinfo!.deviceId);
		}

		[Theory]
		[InlineData("{\"system\":{\"get_sysinfo\":{}}}")]
		public async Task SendAndReceiveMessageTest(string message)
		{
			var response = await _sut.SendAndReceiveMessageAsync(message);

			Assert.NotNull(response);
			Assert.NotEmpty(response);
			Assert.StartsWith("{", response);
			Assert.Matches(
				@"^{""system"":{""get_sysinfo"":{""sw_ver"":"".+?"",""hw_ver"":"".+?"",""model"":"".+?"",""deviceId"":""[0-9A-Fa-f]{40}"",""oemId"":""[0-9A-Fa-f]{32}"",""hwId"":""[0-9A-Fa-f]{32}"",""rssi"":[-\d]+,""longitude_i"":[-\d]+,""latitude_i"":[-\d]+,""alias"":"".+?"",""status"":"".+?"",""mic_type"":"".+?"",""feature"":"".+?"",""mac"":""[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}:[0-9A-Fa-f]{2}"",""updating"":[01],""led_off"":[01],""relay_state"":[01],""on_time"":\d+,""active_mode"":"".+?"",""icon_hash"":"""",""dev_name"":"".+?"",""next_action"":{""type"":[-\d]+},""ntc_state"":[01],""err_code"":[-\d]+}}}$",
				response);
		}

		[Theory]
		[InlineData(208, 242, 129, 248, 139, 255, 154, 247, 213, 239, 148, 182, 209, 180, 192, 159, 236, 149, 230, 143, 225, 135, 232, 202, 240, 139, 246, 139, 246)]
		public async Task ClientBroadcastAndReceiveBytesTest(params int[] requestInts)
		{
			var requestBytes = requestInts.Select(i => (byte)i).ToArray();

			var responsebytes = await _sut.SendAndReceiveBytesAsync(requestBytes);

			Assert.NotNull(responsebytes);
			Assert.NotEmpty(responsebytes);
		}
	}
}
