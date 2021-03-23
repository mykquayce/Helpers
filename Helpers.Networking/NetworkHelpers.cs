using Helpers.Networking.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
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
			var reply = await ping.SendPingAsync(ipAddress, timeout: 10);
			return reply.Status;
		}

		{
		}

		public static Models.ArpResultsDictionary RunArpCommand()
		{
			var output = RunCommand("arp", "-a");
			return Models.ArpResultsDictionary.Parse(output);
		}

		public static string RunCommand(string fileName, string? arguments = default)
		{
			using var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = arguments ?? string.Empty,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
				},
			};

			process.Start();

			return process.StandardOutput.ReadToEnd();
		}

		public static IPAddress IPAddressFromPhysicalAddress(PhysicalAddress physicalAddress)
		{
			foreach (var (_, results) in RunArpCommand())
			{
				foreach (var (mac, ip, _) in results)
				{
					if (mac.Equals(physicalAddress))
					{
						return ip;
					}
				}
			}

			throw new KeyNotFoundException($"{physicalAddress} not found in ARP table")
			{
				Data = { [nameof(physicalAddress)] = physicalAddress, },
			};
		}

		public static PhysicalAddress PhysicalAddressFromIPAddress(IPAddress ipAddress)
		{
			PingAsync(ipAddress).GetAwaiter().GetResult();

			foreach (var (_, results) in RunArpCommand())
			{
				foreach (var (mac, ip, _) in results)
				{
					if (ip.Equals(ipAddress))
					{
						return mac;
					}
				}
			}

			throw new KeyNotFoundException($"{ipAddress} not found in ARP table")
			{
				Data = { [nameof(ipAddress)] = ipAddress, },
			};
		}
	}
}
