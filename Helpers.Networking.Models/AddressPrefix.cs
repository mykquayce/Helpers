using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace Helpers.Networking.Models
{
	public record AddressPrefix(IPAddress IPAddress, byte MaskLength)
	{
		public AddressPrefix() : this(IPAddress.Loopback, 32) { }
		public AddressPrefix(string? s) : this(Parse(s)) { }

		public override string ToString() => string.Join('/', IPAddress, MaskLength);

		public IEnumerable<IPAddress> Addresses
		{
			get
			{
				var count = Count;
				var bytes = IPAddress!.GetAddressBytes();
				var start = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

				for (BigInteger a = 0; a < Count; a++)
				{
					var adjustedBytes = (start + a).ToByteArray().Reverse().SkipWhile(b => b == 0).ToArray();
					yield return new IPAddress(adjustedBytes);
				}
			}
		}

		public BigInteger Count
		{
			get
			{
				var length = IPAddress.GetAddressBytes().Length;
				return BigInteger.Pow(2, length * 8 - MaskLength);
			}
		}

		public static AddressPrefix Parse(string? s)
		{
			if (s is null) return new();
			var strings = s.Split('/');
			var ip = IPAddress.Parse(strings[0]);
			var maskLength = strings.Length > 1
				? byte.Parse(strings[1])
				: GetMaxMaskLength(ip);

			return new(ip, maskLength);
		}

		public static byte GetMaxMaskLength(IPAddress ipAddress)
		{
			if (ipAddress is null) throw new ArgumentNullException(nameof(ipAddress));

			return ipAddress.AddressFamily switch
			{
				AddressFamily.InterNetwork => (byte)32,
				AddressFamily.InterNetworkV6 => (byte)128,
				_ => throw new ArgumentOutOfRangeException(nameof(ipAddress), ipAddress, $"Unexpected {nameof(AddressFamily)}: {ipAddress.AddressFamily}"),
			};
		}
	}
}
