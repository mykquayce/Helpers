using Dawn;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Helpers.Networking
{
	public static class NetworkHelpers
	{
		public static IEnumerable<UnicastIPAddressInformation> GetAllBroadcastAddresses()
		{
			foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
				if (nic.OperationalStatus != OperationalStatus.Up) continue;

				var properties = nic.GetIPProperties();

				foreach (var unicast in properties.UnicastAddresses)
				{
					yield return unicast;
				}
			}
		}

		public async static Task<IPStatus> PingAsync(IPAddress ipAddress)
		{
			using var ping = new Ping();
			var reply = await ping.SendPingAsync(ipAddress, timeout: 10_000);
			return reply.Status;
		}

		public async static Task<(IPAddress, IPStatus)> PingAsync(string hostName)
		{
			Guard.Argument(hostName).NotNull().NotEmpty().NotWhiteSpace();
			using var ping = new Ping();
			var reply = await ping.SendPingAsync(hostName, timeout: 10_000);
			return (reply.Address, reply.Status);
		}
	}
}
