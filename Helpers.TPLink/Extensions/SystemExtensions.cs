using System.Text;

namespace System;

public static class SystemExtensions
{
	private static readonly Encoding _encoding = Encoding.UTF8;
	private const byte _initialKey = 0xAB;

	public static byte[] Encode(this string s) => _encoding.GetBytes(s);
	public static string Decode(this byte[] bb) => _encoding.GetString(bb);

	public static byte[] Encrypt(this byte[] bb)
	{
		var result = new byte[bb.Length];
		var key = _initialKey;
		for (var a = 0; a < result.Length; a++)
		{
			key = result[a] = (byte)(key ^ bb[a]);
		}
		return result;
	}

	public static byte[] Decrypt(this byte[] bb)
	{
		var result = new byte[bb.Length];
		var key = _initialKey;
		for (var a = 0; a < result.Length; a++)
		{
			result[a] = (byte)(key ^ bb[a]);
			key = bb[a];
		}
		return result;
	}
}
