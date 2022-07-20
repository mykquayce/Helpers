using Dawn;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace Helpers.Networking.Models;

public record AddressPrefix(IPAddress IPAddress, byte MaskLength)
	: IParsable<AddressPrefix>
{
	public AddressPrefix() : this(IPAddress.Loopback, 32) { }
	public AddressPrefix(string s) : this(Parse(s, null)) { }

	public override string ToString() => string.Join('/', IPAddress, MaskLength);

	public IEnumerable<IPAddress> Addresses
	{
		get
		{
			var count = Count;
			var bytes = IPAddress!.GetAddressBytes();
			var start = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

			for (BigInteger a = 0; a < count; a++)
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
			var length = IPAddress.GetAddressBytes().Length * 8;
			return BigInteger.Pow(2, length - MaskLength);
		}
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

	#region iparsable implementation
	public static AddressPrefix Parse(string s, IFormatProvider? provider)
		=> TryParse(s, provider, out var prefix)
			? prefix
			: throw new ArgumentOutOfRangeException(nameof(s), s, "Failed to find a prefix in " + s);

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out AddressPrefix result)
	{
		Guard.Argument(s!).NotNull().NotEmpty().NotWhiteSpace().Contains('/');
		var (ipString, maskLengthString) = s!.Split('/');

		if (IPAddress.TryParse(ipString, out var ip)
			&& byte.TryParse(maskLengthString, provider, out var maskLength))
		{
			result = new(ip, maskLength);
			return true;
		}

		result = null!;
		return false;
	}
	#endregion iparsable implementation
}
