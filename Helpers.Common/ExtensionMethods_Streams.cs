using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Helpers.Common
{
	public static partial class ExtensionMethods
	{
		public static IEnumerable<string> ReadLines(this Stream stream)
		{
			var reader = new StreamReader(stream, Encoding.UTF8);
			return reader.ReadLines();
		}

		public static IAsyncEnumerable<string> ReadLinesAsync(this Stream stream)
		{
			var reader = new StreamReader(stream, Encoding.UTF8);
			return reader.ReadLinesAsync();
		}

		public static IEnumerable<string> ReadLines(this StreamReader reader)
		{
			string? line;
			while ((line = reader.ReadLine()) is not null)
			{
				yield return line;
			}
		}

		public async static IAsyncEnumerable<string> ReadLinesAsync(this StreamReader reader)
		{
			string? line;
			while ((line = await reader.ReadLineAsync()) is not null)
			{
				yield return line;
			}
		}
	}
}
