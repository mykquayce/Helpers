using System.Numerics;

namespace System.Net;

public static class IPAddressExtensions
{
	public static UInt128 GetUInt128(this IPAddress ip)
		=> (UInt128)new BigInteger(ip.GetAddressBytes(), isUnsigned: true, isBigEndian: true);
}
