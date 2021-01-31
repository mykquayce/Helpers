using System.Net;

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
