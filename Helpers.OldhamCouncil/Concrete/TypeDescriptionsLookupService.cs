using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Helpers.OldhamCouncil.Concrete
{
	public class TypeDescriptionsLookupService<T> : ITypeDescriptionsLookupService<T>
	{
		private readonly IDictionary<string, T> _dictionary = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

		public TypeDescriptionsLookupService()
		{
			var type = typeof(T);
			var fields = type.GetFields();

			foreach (var field in fields)
			{
				var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);

				foreach (var attribute in attributes)
				{
					if (attribute is not DescriptionAttribute descriptionAttribute)
					{
						continue;
					}

					var key = descriptionAttribute.Description;
					var value = (T)field.GetValue(default)!;

					_dictionary.Add(key, value);
				}
			}
		}

		#region IReadOnlyDictionary implementation
		public T this[string key] => _dictionary[key];
		public IEnumerable<string> Keys => _dictionary.Keys;
		public IEnumerable<T> Values => _dictionary.Values;
		public int Count => _dictionary.Count;
		public bool ContainsKey(string key) => _dictionary.ContainsKey(key);
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => _dictionary.GetEnumerator();
		public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) => _dictionary.TryGetValue(key, out value);
		IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
		#endregion IReadOnlyDictionary implementation
	}
}
