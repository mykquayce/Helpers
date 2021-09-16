using Dawn;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Helpers.GlobalCache.Concrete
{
	public partial class Client : IClient
	{
		private readonly IPAddress _broadcastIPAddress;
		private readonly ushort _receivePort;

		#region constructors
		public Client(IOptions<Config> options) : this(options.Value) { }
		public Client(Config config)
		{
			Guard.Argument(config).NotNull();
			_broadcastIPAddress = Guard.Argument(config.BroadcastIPAddress).NotNull().Value;
			_receivePort = Guard.Argument(config.ReceivePort).Positive().Value;
		}
		#endregion constructors

		public async IAsyncEnumerable<Models.Beacon> DiscoverAsync()
		{
			using var udpClient = new Helpers.Networking.Clients.Concrete.UdpClient(_broadcastIPAddress, _receivePort);
			var beacons = udpClient.DiscoverAsync().Select(Models.Beacon.Parse).Distinct();
			await foreach (var beacon in beacons) yield return beacon;
		}
	}
}
