﻿using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Extensions
{
	public static class UnicastIPAddressInformationExtensions
	{
		public static IPAddress GetBroadcastAddress(this UnicastIPAddressInformation unicast)
		{
			if (unicast is null) throw new ArgumentNullException(nameof(unicast));

			var ip = unicast.Address.GetAddressBytes();
			var mask = unicast.IPv4Mask.GetAddressBytes();
			var broadcast = new byte[ip.Length];

			for (var a = 0; a < ip.Length; a++)
			{
				broadcast[a] = (byte)(ip[a] | (byte)~mask[a]);
			}

			return new IPAddress(broadcast);
		}
	}
}
