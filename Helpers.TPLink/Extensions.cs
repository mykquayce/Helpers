using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Helpers.TPLink
{
	public static class Extensions
	{
		private static readonly Encoding _encoding = Encoding.UTF8;
		private const byte _initialKey = 0xAB;

		#region serialize
		public static string Serialize(this object o) => JsonSerializer.Serialize(o);
		public static T Deserialize<T>(this string s) => JsonSerializer.Deserialize<T>(s)
			?? throw new Exception($"Failed to deserialize {s} to {typeof(T).Name}")
			{
				Data =
				{
					[nameof(s)] = s,
					["type"] = typeof(T),
				},
			};
		#endregion serialize

		#region encode
		public static byte[] Encode(this string s) => _encoding.GetBytes(s);
		public static string Decode(this byte[] bb) => _encoding.GetString(bb);
		#endregion encode

		#region encrypt
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
		#endregion encrypt

		#region hex
		public static string ToHex(this byte b) => Convert.ToString(b, toBase: 16);
		public static IEnumerable<string> ToHex(this IEnumerable<byte> bb) => bb.Select(ToHex);
		#endregion hex

		#region decimal
		public static byte ToDecimal(this string s) => Convert.ToByte(s, fromBase: 16);
		public static IEnumerable<byte> ToDecimal(this IEnumerable<string> ss) => ss.Select(ToDecimal);
		#endregion decimal
	}
}
