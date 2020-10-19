using System.Collections.Generic;

namespace Helpers.TPLink.Models.Extensions
{
	public static class DictionaryExtensions
	{
		public static IDictionary<TKey, TValue> SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary.TryAdd(key, value))
			{
				return dictionary;
			}

			dictionary[key] = value;

			return dictionary;
		}
	}
}
