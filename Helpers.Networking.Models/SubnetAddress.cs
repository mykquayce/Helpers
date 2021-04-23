using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;

namespace Helpers.Networking.Models
{
	public record SubnetAddress(IPAddress? IPAddress, byte? Mask)
	{
		public SubnetAddress() : this(default, default) { }

		public override string ToString()
		{
			if (IPAddress is null) return string.Empty;
			if (Mask is null) return IPAddress.ToString();
			return $"{IPAddress}/{Mask}";
		}

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
				var length = IPAddress?.GetAddressBytes().Length ?? 0;
				var byteLength = length * 8;
				var mask = Mask ?? 0;
				return BigInteger.Pow(2, byteLength - mask);
			}
		}

		public static SubnetAddress Parse(string s)
		{
			var strings = s.Split('/');
			var ip = IPAddress.Parse(strings[0]);
			var mask = strings.Length > 1
				? byte.Parse(strings[1])
				: default(byte?);

			var subnetAddress = new SubnetAddress(ip, mask);
			return subnetAddress;
		}
	}
}
