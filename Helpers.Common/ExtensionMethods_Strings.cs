using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Helpers.Common
{
	public static partial class ExtensionMethods
	{
		public static bool Overlaps(this string left, string right) => right.GetBeginnings().Any(left.EndsWith);

		public static IEnumerable<string> GetBeginnings(this string s) => GetIncrementals(s).Take(s.Length - 1);

		public static IEnumerable<string> GetIncrementals(this string s)
		{
			for (var a = 1; a <= s.Length; a++)
			{
				yield return s[..a];
			}
		}

		public static IEnumerable<string> GetLines(this Stream stream, string separator = "\r\n", int bufferSize = 20)
		{
			var piecesQueue = new Queue<string>();
			using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
			char[] chars;
			do
			{
				chars = reader.ReadChars(bufferSize);
				var s = new string(chars);

				while (s.Overlaps(separator))
				{
					Array.Resize(ref chars, newSize: chars.Length + 1);
					chars[^1] = reader.ReadChar();
					s = new string(chars);
				}

				var lines = s.Split(separator);

				if (lines.Length > 1)
				{
					yield return string.Concat(piecesQueue.AsEnumerable().Append(lines[0]));
					piecesQueue.Clear();
				}

				if (lines.Length > 2)
				{
					foreach (var line in lines[1..^1])
					{
						yield return line;
					}
				}

				var last = lines[^1];
				if (!string.IsNullOrWhiteSpace(last))
				{
					piecesQueue.Enqueue(last);
				}
			}
			while (chars.Length > 0);
		}
	}
}
