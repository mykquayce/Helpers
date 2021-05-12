using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.Elgato.Concrete
{
	public sealed class ElgatoService : IElgatoService
	{
		private const int _port = 9_123;
		private readonly Helpers.SSH.Services.ISSHService _sshService;
		private readonly IElgatoClient _client = new ElgatoClient();
		private readonly static IAddressesCache _cache = new AddressesCache();

		public ElgatoService(Helpers.SSH.Services.ISSHService sshService)
		{
			_sshService = sshService;
		}

		public void Dispose() => _client?.Dispose();

		public async Task<IPAddress> GetIPAddressAsync(PhysicalAddress physicalAddress)
		{
			if (_cache.TryGet(physicalAddress, out var ipAddress))
			{
				return ipAddress!;
			}

			var lease = await _sshService.GetLeaseByPhysicalAddressAsync(physicalAddress);
			_cache.Set(physicalAddress, lease.IPAddress, lease.Expiration.AddMinutes(-5));

			return lease.IPAddress;
		}

		public async Task<Models.AccessoryInfoObject> GetAccessoryInfoAsync(PhysicalAddress physicalAddress)
		{
			var ipAddress = await GetIPAddressAsync(physicalAddress);
			var endPoint = new IPEndPoint(ipAddress!, _port);
			return await _client.GetAccessoryInfoAsync(endPoint);
		}

		public async Task<Models.MessageObject.LightObject> GetLightAsync(PhysicalAddress physicalAddress)
		{
			var ipAddress = await GetIPAddressAsync(physicalAddress);
			var endPoint = new IPEndPoint(ipAddress!, _port);
			return await _client.GetLightAsync(endPoint);
		}

		public async Task SetLightAsync(PhysicalAddress physicalAddress, Models.MessageObject.LightObject light)
		{
			var ipAddress = await GetIPAddressAsync(physicalAddress);
			var endPoint = new IPEndPoint(ipAddress!, _port);
			await _client.SetLightAsync(endPoint, light);
		}

		public async Task ToggleLightPowerStateAsync(PhysicalAddress physicalAddress)
		{
			var ipAddress = await GetIPAddressAsync(physicalAddress);
			var endPoint = new IPEndPoint(ipAddress!, _port);

			var old = await _client.GetLightAsync(endPoint);

			var @new = old with { on = old.on == 1 ? (byte)0 : (byte)1, };

			await _client.SetLightAsync(endPoint, @new);
		}
	}
}
