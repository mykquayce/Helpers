namespace System.Collections.Generic;

public static class DictionaryExtensions
{
	public static TValue GetFirst<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params TKey[] keys)
	{
		foreach (var key in keys)
		{
			if (dictionary.TryGetValue(key, out var result))
			{
				return result;
			}
		}

		throw new KeyNotFoundException();
	}
}
