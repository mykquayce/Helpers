using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.TPLink.Concrete
{
	public class TPLinkClient : ITPLinkClient
	{
		public record Config(ushort Port)
		{
			public const ushort DefaultPort = 9_999;

			public Config() : this(DefaultPort) { }

			public static Config Defaults => new();
		}

		private readonly ushort _port;
		private readonly static object _discoveryObject = new { system = new { get_sysinfo = new { }, }, };
		private readonly static object _getRealtimeDataObject = new { system = new { get_sysinfo = new { }, }, emeter = new { get_realtime = new { }, }, };

		public TPLinkClient(IOptions<Config> options) : this(options.Value) { }
		public TPLinkClient(Config config) : this(config.Port) { }
		public TPLinkClient(ushort port = Config.DefaultPort) => _port = port;

		public static IEnumerable<IPAddress> LocalIPAddresses => Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses().Select(ip => ip.Address);

		public async IAsyncEnumerable<Models.Device> DiscoverAsync()
		{
			var endPoint = new IPEndPoint(IPAddress.Broadcast, _port);
			var responses = SendAndReceiveAsync(endPoint, _discoveryObject);

			await foreach (var (ip, response) in responses)
			{
				var sysInfo = (Models.SystemInfo)response!.system.get_sysinfo;
				yield return new(sysInfo.Alias, ip, sysInfo.PhysicalAddress);
			}
		}

		public async Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip)
		{
			var endPoint = new IPEndPoint(ip, _port);
			var (_, response) = await SendAndReceiveAsync(endPoint, _getRealtimeDataObject).FirstAsync();
			return (Models.RealtimeData)response!.emeter!.get_realtime;
		}

		private async static IAsyncEnumerable<(IPAddress, Models.Generated.ResponseObject)> SendAndReceiveAsync(IPEndPoint endPoint, object request)
		{
			var localIPs = LocalIPAddresses.ToList();
			var requestBytes = request.Serialize().Encode().Encrypt();
			using var cts = new CancellationTokenSource(millisecondsDelay: 3_000);
			using var udpClient = new UdpClient(endPoint.Port);
			await udpClient.SendAsync(requestBytes, requestBytes.Length, endPoint);

			while (!cts.Token.IsCancellationRequested)
			{
				var task = await Task.WhenAny(
					udpClient.ReceiveAsync(),
					Task.Delay(1_000, cts.Token));

				if (task is not Task<UdpReceiveResult> myTask) continue;
				var result = await myTask;
				var ip = result.RemoteEndPoint.Address;
				if (localIPs.Contains(ip)) continue;
				var response = result.Buffer.Decrypt().Decode().Deserialize<Models.Generated.ResponseObject>();
				yield return (ip, response);
			}
		}
	}
}
