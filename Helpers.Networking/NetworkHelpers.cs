﻿using Helpers.Networking.Extensions;
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

		public async static IAsyncEnumerable<(IPAddress, IPStatus)> PingEntireNetworkAsync()
		{
			foreach (var unicast in GetAllBroadcastAddresses())
			{
				var broadcast = unicast.GetBroadcastAddress();
				var status = await PingAsync(broadcast);
				yield return (broadcast, status);
			}
		}

		public static Models.ArpResultsCollection RunArpCommand()
		{
			var output = RunCommand("arp", "-a");
			return Models.ArpResultsCollection.Parse(output);
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
	}
}
