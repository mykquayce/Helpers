using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class Scratch
	{
		[Fact]
		public async Task Test1()
		{
			//var hex = "020000010000000000000000463cb5d3";

			var bytes = new byte[16] { 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46, 0x3c, 0xb5, 0xd3, };
			var key = (byte)0xAB;
			var o = new { system = new { get_sysinfo = default(object), }, };
			var json = JsonSerializer.Serialize(o);
			//var bytes = Encoding.UTF8.GetBytes(json);
			var encrypted = new byte[bytes.Length];

			for (var a = 0; a < bytes.Length; a++)
			{
				encrypted[a] = key = (byte)(key ^ bytes[a]);
			}

			using var client = new TcpClient(AddressFamily.InterNetwork);

			var ipAddress = new IPAddress(new byte[4] { 255, 255, 255, 255, });
			var endPoint = new IPEndPoint(ipAddress, 9_999);

			client.Connect(endPoint);

			//var result = client.BeginConnect(ipAddress, 9_999, default, default);
			//var success = result.AsyncWaitHandle.WaitOne(millisecondsTimeout: 10_000);

			//Assert.True(success);
			Assert.True(client.Connected);

			await using var stream = client.GetStream();

			stream.Write(encrypted, offset: 0, count: encrypted.Length);

			var buffer = new byte[10_240];
			var count = stream.Read(buffer);
		}

		[Theory]
		[InlineData(@"{
        ""system"": {""get_sysinfo"": None},
        ""emeter"": {""get_realtime"": None},
        ""smartlife.iot.dimmer"": {""get_dimmer_parameters"": None},
        ""smartlife.iot.common.emeter"": {""get_realtime"": None},
        ""smartlife.iot.smartbulb.lightingservice"": {""get_light_state"": None},
    }")]
		public async Task Test2(string json)
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			var encrypted = new byte[bytes.Length];
		}
	}
}
