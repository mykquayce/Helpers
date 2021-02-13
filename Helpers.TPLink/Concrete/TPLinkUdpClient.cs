using Dawn;
using Helpers.TPLink.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.TPLink.Concrete
{
	public class TPLinkUdpClient : ITPLinkUdpClient
	{
		#region config poco
		public record Config(int? MillisecondsTimeout = 5_000, ushort? Port = 9_999)
		{
			public Config() : this(default, default) { }
		}
		#endregion config poco

		private readonly ushort _port;
		private readonly int _millisecondsTimeout;
		private readonly IEncryptionService _encryptionService;
		private readonly Encoding _encoding = Encoding.UTF8;

		#region constructors
		public TPLinkUdpClient(IOptions<Config> options, IEncryptionService encryptionService)
			: this(options.Value, encryptionService)
		{ }

		public TPLinkUdpClient(Config config, IEncryptionService encryptionService)
			: this(config.MillisecondsTimeout ?? default, config.Port ?? default, encryptionService)
		{ }

		public TPLinkUdpClient(int millisecondsTimeout, ushort port, IEncryptionService encryptionService)
		{
			_millisecondsTimeout = Guard.Argument(() => millisecondsTimeout).Positive().Value;
			_port = Guard.Argument(() => port).Positive().Value;
			_encryptionService = Guard.Argument(() => encryptionService).NotNull().Value;
		}
		#endregion constructors

		public async Task<DiscoveryResponseObject.ResultObject> DiscoverAsync()
		{
			var requestBytes = new byte[] { 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 70, 60, 181, 211, };

			var responseBytes = await SendAndReceiveBytesAsync(requestBytes);
			await using var responseStream = new MemoryStream(responseBytes[16..]);
			var response = await JsonSerializer.DeserializeAsync<DiscoveryResponseObject>(responseStream);
			return response!.result!;
		}

		public async Task<ResponseDataObject.EmeterObject.RealtimeObject> GetRealtimeAsync()
		{
			var requestObject = new
			{
				system = new { get_sysinfo = new object(), },
				emeter = new { get_realtime = new object(), },
			};

			var responseObject = await SendAndReceiveObjectAsync<ResponseDataObject>(requestObject);

			return responseObject!.emeter!.get_realtime!;
		}

		public async Task<T> SendAndReceiveObjectAsync<T>(object requestObject)
		{
			var requestJsonObject = JsonSerializer.Serialize(requestObject);
			var responseJsonObject = await SendAndReceiveMessageAsync(requestJsonObject);
			return JsonSerializer.Deserialize<T>(responseJsonObject)!;
		}

		public async Task<string> SendAndReceiveMessageAsync(string message)
		{
			var messageBytes = _encoding.GetBytes(message);
			var encryptedMessageBytes = _encryptionService.Encrypt(messageBytes);
			var responseBytes = await SendAndReceiveBytesAsync(encryptedMessageBytes);
			var decryptedResponseBytes = _encryptionService.Decrypt(responseBytes);
			return _encoding.GetString(decryptedResponseBytes);
		}

		public async Task<byte[]> SendAndReceiveBytesAsync(byte[] bytes)
		{
			var toEndPoint = new IPEndPoint(IPAddress.Broadcast, _port);

			using var udpClient = new UdpClient();
			await udpClient.SendAsync(bytes, bytes.Length, toEndPoint);

			var task = await Task.WhenAny(
				udpClient.ReceiveAsync(),
				Task.Delay(_millisecondsTimeout));

			if (task is not Task<UdpReceiveResult> resultTask)
			{
				throw new TimeoutException();
			}

			var result = await resultTask;
			return result.Buffer;
		}
	}
}
