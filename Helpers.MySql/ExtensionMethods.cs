using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.MySql
{
	public static class ExtensionMethods
	{
		public static IDictionary<string, string> ToDictionary(this string s, char separator1 = ';', char separator2 = '=')
		{
			return (
				from pair in s.Split(separator1, StringSplitOptions.RemoveEmptyEntries)
				select pair.Split(separator2)
			).ToDictionary(a => a[0], a => a[1], StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
