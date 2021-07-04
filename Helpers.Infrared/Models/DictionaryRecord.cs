using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Helpers.Infrared.Models
{
	public record DictionaryRecord<TKey, TValue>(IDictionary<TKey, TValue> Dictionary) : IDictionary<TKey, TValue>
		where TKey : notnull
	{
		public DictionaryRecord() : this(new Dictionary<TKey, TValue>()) { }

		public TValue this[TKey key] { get => Dictionary[key]; set => Dictionary[key] = value; }
		public ICollection<TKey> Keys => Dictionary.Keys;
		public ICollection<TValue> Values => Dictionary.Values;
		public int Count => Dictionary.Count;
		public bool IsReadOnly => Dictionary.IsReadOnly;

		public void Add(TKey key, TValue value) => Dictionary.Add(key, value);
		public void Add(KeyValuePair<TKey, TValue> item) => Dictionary.Add(item);
		public void Clear() => Dictionary.Clear();
		public bool Contains(KeyValuePair<TKey, TValue> item) => Dictionary.Contains(item);
		public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();
		public bool Remove(TKey key) => Dictionary.Remove(key);
		public bool Remove(KeyValuePair<TKey, TValue> item) => Dictionary.Remove(item);
		public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);
		IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();
	}
}
