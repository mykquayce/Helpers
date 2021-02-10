using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
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
			var config = new Concrete.TPLinkUdpClient.Config { MillisecondsTimeout = 5_000, Port = 20_002, };

			_sut = new Concrete.TPLinkUdpClient(Options.Create(config), encryptionService);
		}

		[Fact]
		public async Task DiscoverTest()
		{
			// Act
			var response = await _sut.DiscoverAsync();

			// Assert
			Assert.NotNull(response);
			Assert.NotNull(response.ip);
			Assert.NotNull(response.mac);
			Assert.NotNull(response.device_id);
			Assert.NotNull(response.owner);
			Assert.NotNull(response.device_type);
			Assert.NotNull(response.device_model);
			Assert.NotNull(response.hw_ver);
			Assert.NotNull(response.factory_default);
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

		[Theory]
		[InlineData("255.255.255.255", 20_002)]
		public async Task Discover(string ipString, ushort port)
		{
			var bytes = new byte[] { 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 70, 60, 181, 211, };
			var toEndPoint = new IPEndPoint(IPAddress.Parse(ipString), port);

			using var udpClient = new UdpClient();
			await udpClient.SendAsync(bytes, bytes.Length, toEndPoint);

			var task = await Task.WhenAny(
				udpClient.ReceiveAsync(),
				Task.Delay(100));

			if (task is not Task<UdpReceiveResult> resultTask)
			{
				throw new TimeoutException();
			}

			var result = await resultTask;

			var responseBytes = result.Buffer[16..];
			var responseJson = System.Text.Encoding.UTF8.GetString(responseBytes);
			var responseObject = JsonSerializer.Deserialize<ResponseObject>(responseJson);
		}
	}


	public class ResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public ResultObject? result { get; init; }
		public int? error_code { get; init; }

		public class ResultObject
		{
			public string? ip { get; init; }
			public string? mac { get; init; }
			public string? device_id { get; init; }
			public string? owner { get; init; }
			public string? device_type { get; init; }
			public string? device_model { get; init; }
			public string? hw_ver { get; init; }
			public bool? factory_default { get; init; }
			public ManagementEncryptionSchemeObject? mgt_encrypt_schm { get; init; }

			public class ManagementEncryptionSchemeObject
			{
				public bool? is_support_https { get; init; }
				public string? encrypt_type { get; init; }
				public int? http_port { get; init; }
			}
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
