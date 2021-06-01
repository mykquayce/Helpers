using Dawn;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Helpers.TPLink.Concrete
{
	public class TPLinkClient : ITPLinkClient
	{
		private readonly object _discoveryObject = new { system = new { get_sysinfo = new { }, }, };
		private readonly object _getDataObject = new { system = new { get_sysinfo = new { }, }, emeter = new { get_realtime = new { }, }, };

		private readonly IDeviceCache _deviceCache = new DeviceCache();
		private readonly IEncryptionService _encryptionService;
		private readonly IPEndPoint _broadcastEndPoint;
		private readonly Encoding _encoding = Encoding.UTF8;

		public record Config(
			[property: JsonConverter(typeof(Helpers.Json.Converters.JsonIPEndPointConverter))] IPEndPoint BroadcastEndPoint)
		{
			public static readonly IPAddress DefaultBroadcastIPAddress = IPAddress.Broadcast;
			public const int DefaultPort = 9_999;
			public static readonly IPEndPoint DefaultBroadcastEndPoint = new(DefaultBroadcastIPAddress, DefaultPort);
		}

		#region constructors
		public TPLinkClient(IDeviceCache deviceCache, IEncryptionService encryptionService, IOptions<Config> options) : this(deviceCache, encryptionService, options.Value) { }
		public TPLinkClient(IDeviceCache deviceCache, IEncryptionService encryptionService, Config value) : this(deviceCache, encryptionService, value.BroadcastEndPoint) { }
		public TPLinkClient(IDeviceCache deviceCache, IEncryptionService encryptionService, IPEndPoint? endPoint = default)
		{
			_deviceCache = Guard.Argument(() => deviceCache).NotNull().Value;
			_encryptionService = encryptionService;
			_broadcastEndPoint = endPoint ?? Config.DefaultBroadcastEndPoint;
		}
		#endregion constructors

		private static IEnumerable<IPAddress> LocalIPAddresses => Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses().Select(ip => ip.Address);

		#region Discover
		public async IAsyncEnumerable<Models.Device> DiscoverAsync()
		{
			var localIPAddresses = LocalIPAddresses.ToList();
			var results = SendAndReceiveAsync(_broadcastEndPoint, _discoveryObject);

			await foreach (var result in results)
			{
				var ipAddress = result.RemoteEndPoint.Address;
				if (localIPAddresses.Contains(ipAddress)) continue;

				var decrypted = _encryptionService.Decrypt(result.Buffer);
				var json = _encoding.GetString(decrypted);
				var response = JsonSerializer.Deserialize<Models.Generated.ResponseObject>(json);
				var sysInfo = (Models.SystemInfo)response!.system.get_sysinfo;
				yield return new(sysInfo.Alias, ipAddress, sysInfo.PhysicalAddress);
			}
		}
		#endregion Discover

		#region GetRealtimeData
		public delegate bool TryGetDelegate<T>(T value, [MaybeNullWhen(false)] out Models.Device device);
		public Task<Models.RealtimeData> GetRealtimeDataAsync(string alias) => GetRealtimeDataAsync(_deviceCache.TryGetValue, alias);
		public Task<Models.RealtimeData> GetRealtimeDataAsync(IPAddress ip) => GetRealtimeDataAsync(_deviceCache.TryGetValue, ip);
		public Task<Models.RealtimeData> GetRealtimeDataAsync(PhysicalAddress mac) => GetRealtimeDataAsync(_deviceCache.TryGetValue, mac);

		public async Task<Models.RealtimeData> GetRealtimeDataAsync<T>(TryGetDelegate<T> tryGet, T value)
		{
			if (!tryGet(value, out var device))
			{
				await foreach (var item in DiscoverAsync())
				{
					_deviceCache.Add(item);
				}

				if (!tryGet(value, out device))
				{
					throw new KeyNotFoundException(value + " not found");
				}
			}

			return await GetRealtimeDataAsync(device);
		}

		public async Task<Models.RealtimeData> GetRealtimeDataAsync(Models.Device device)
		{
			var endPoint = new IPEndPoint(device.IPAddress, _broadcastEndPoint.Port);
			var result = await SendAndReceiveAsync(endPoint, _getDataObject).FirstAsync();
			var decrypted = _encryptionService.Decrypt(result.Buffer);
			var json = _encoding.GetString(decrypted);
			var response = JsonSerializer.Deserialize<Models.Generated.ResponseObject>(json);
			var realtime = (Models.RealtimeData)response!.emeter!.get_realtime;
			return realtime;
		}
		#endregion GetRealtimeData

		private IAsyncEnumerable<UdpReceiveResult> SendAndReceiveAsync(IPEndPoint endPoint, object o)
		{
			var json = JsonSerializer.Serialize(o);
			var encoded = _encoding.GetBytes(json);
			var encrypted = _encryptionService.Encrypt(encoded);
			return SendAndReceiveAsync(endPoint, encrypted);
		}

		private async static IAsyncEnumerable<UdpReceiveResult> SendAndReceiveAsync(IPEndPoint endPoint, byte[] requestBytes)
		{
			using var client = new UdpClient(endPoint.Port);
			await client.SendAsync(requestBytes, requestBytes.Length, endPoint);

			await Task.Delay(millisecondsDelay: 1_000);

			while (true)
			{
				var task = await Task.WhenAny(
					client.ReceiveAsync(),
					Task.Delay(1_000));

				if (task is not Task<UdpReceiveResult> resultTask)
				{
					yield break;
				}

				var result = await resultTask;
				yield return result;
			}
		}
	}
}
