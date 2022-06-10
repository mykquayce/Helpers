using System.Collections;

namespace System;

public static class DictionaryExtensions
{
	public static IDictionary TryAdd(this IDictionary dictionary, string key, object value)
	{
		if (dictionary.Contains(key))
		{
			dictionary[key] = value;
		}
		else
		{
			dictionary.Add(key, value);
		}

		return dictionary;
	}
}
