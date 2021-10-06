using System.Collections;
using System.Collections.Generic;

namespace Helpers.MySql;

public class ExceptionDictionary : IDictionary<ExceptionTypes, int>
{
	private readonly IDictionary<ExceptionTypes, int> _dictionary = new Dictionary<ExceptionTypes, int>();

	public int this[ExceptionTypes key]
	{
		get
		{
			return _dictionary.TryGetValue(key, out var result)
				? result
				: 0;
		}
		set
		{
			if (_dictionary.ContainsKey(key))
			{
				_dictionary[key] = value;
			}
			else
			{
				_dictionary.Add(key, value);
			}
		}
	}

	public ICollection<ExceptionTypes> Keys => _dictionary.Keys;
	public ICollection<int> Values => _dictionary.Values;
	public int Count => _dictionary.Count;
	public bool IsReadOnly => _dictionary.IsReadOnly;
	public void Add(ExceptionTypes key, int value) => _dictionary.Add(key, value);
	public void Add(KeyValuePair<ExceptionTypes, int> item) => Add(item);
	public void Clear() => _dictionary.Clear();
	public bool Contains(KeyValuePair<ExceptionTypes, int> item) => _dictionary.Contains(item);
	public bool ContainsKey(ExceptionTypes key) => _dictionary.ContainsKey(key);
	public void CopyTo(KeyValuePair<ExceptionTypes, int>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
	public IEnumerator<KeyValuePair<ExceptionTypes, int>> GetEnumerator() => _dictionary.GetEnumerator();
	public bool Remove(ExceptionTypes key) => _dictionary.Remove(key);
	public bool Remove(KeyValuePair<ExceptionTypes, int> item) => _dictionary.Remove(item);
	public bool TryGetValue(ExceptionTypes key, out int value) => _dictionary.TryGetValue(key, out value);
	IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
}
