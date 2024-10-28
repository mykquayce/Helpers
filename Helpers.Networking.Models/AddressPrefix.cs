using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Numerics;

namespace Helpers.Networking.Models;

public record AddressPrefix(IPAddress IPAddress, byte MaskLength)
	: IParsable<AddressPrefix>
{
	private BigInteger? _count;
	private byte? _length;
	private UInt128? _mask;

	public AddressPrefix() : this(IPAddress.Loopback, 32) { }

	public override string ToString() => string.Join('/', IPAddress, MaskLength);

	public IEnumerable<IPAddress> Addresses
	{
		get
		{
			var bytes = IPAddress!.GetAddressBytes();
			var start = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

			for (BigInteger a = 0; a < Count; a++)
			{
				var adjustedBytes = (start + a).ToByteArray().Reverse().SkipWhile(b => b == 0).ToArray();
				yield return new IPAddress(adjustedBytes);
			}
		}
	}

	public BigInteger Count => _count ??= GetCount();
	public byte Length => _length ??= (byte)(IPAddress.GetAddressBytes().Length * 8);
	public UInt128 Mask => _mask ??= GetMask();

	public bool Contains(IPAddress other) => (Mask & other.GetUInt128()) == Mask;

	private BigInteger GetCount()
	{
		var length = IPAddress.GetAddressBytes().Length * 8;
		return BigInteger.Pow(2, length - MaskLength);
	}

	public UInt128 GetMask()
	{
		var left = IPAddress.GetUInt128();
		UInt128 right;
		{
			right = (UInt128)(Math.Pow(2, MaskLength + 1) - 1) << (Length - MaskLength);
		}

		return left & right;
	}

	#region iparsable implementation
	public static AddressPrefix Parse(string s, IFormatProvider? provider)
		=> TryParse(s, provider, out var prefix)
			? prefix
			: throw new ArgumentOutOfRangeException(nameof(s), s, "Failed to find a prefix in " + s);

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out AddressPrefix result)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			result = null!;
			return false;
		}

		var values = s!.Split('/', count: 2);

		if (!IPAddress.TryParse(values[0], out var ip))
		{
		}

		if (values.Length == 1)
		{
			result = new AddressPrefix(ip, (byte)(ip.GetAddressBytes().Length * 8));
			return true;
		}

		if (byte.TryParse(values[1], out var maskLength))
		{
			result = new AddressPrefix(ip, maskLength);
			return true;
		}

		result = null!;
		return false;
	}
	#endregion iparsable implementation
}
