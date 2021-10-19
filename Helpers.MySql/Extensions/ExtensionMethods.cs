namespace System.Collections.Generic;

public static class ExtensionMethods
{
	public static IReadOnlyDictionary<string, string> ToDictionary(this string s, char separator1 = ';', char separator2 = '=')
	{
		var query = from kvp in s.Split(separator1, StringSplitOptions.RemoveEmptyEntries)
					let aa = kvp.Split(separator2)
					let key = aa[0]
					let value = aa[1]
					select new KeyValuePair<string, string>(key, value);

		return query
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
	}
}
