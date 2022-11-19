using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Caching.Memory;

public class EnumerableMemoryCache : IEnumerableMemoryCache
{
	private readonly IMemoryCache _memoryCache;
	private readonly IList<object> _keys;

	public EnumerableMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor)
		: this(optionsAccessor, NullLoggerFactory.Instance)
	{ }

	public EnumerableMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor, ILoggerFactory loggerFactory)
	{
		_memoryCache = new MemoryCache(optionsAccessor, loggerFactory);
		_keys = new List<object>();
	}

	private IEnumerable<KeyValuePair<object, object?>> GetEnumerable()
	{
		for (var a = _keys.Count - 1; a >= 0; a--)
		{
			var ok = _memoryCache.TryGetValue(_keys[a], out var value);

			if (ok)
			{
				yield return new(_keys[a], value);
			}
			else
			{
				_keys.RemoveAt(a);
			}
		}
	}

	#region idictionary implementation
	public object? this[object key] { get => _memoryCache.Get(key); set => Add(key, value); }
	public ICollection<object> Keys => GetEnumerable().Select(kvp => kvp.Key).ToList();
	public ICollection<object?> Values => GetEnumerable().Select(kvp => kvp.Value).ToList();
	public int Count => GetEnumerable().Count();
	public bool IsReadOnly => false;
	public void Add(object key, object? value)
	{
		if (!_keys.Contains(key)) _keys.Add(key);
		_memoryCache.Set(key, value);
	}

	public void Add(KeyValuePair<object, object?> item) => Add(item.Key, item.Value);

	public void Clear()
	{
		_keys.Clear();
		((MemoryCache)_memoryCache).Clear();
	}

	public bool Contains(KeyValuePair<object, object?> item) => _memoryCache.TryGetValue(item.Key, out var value) && Equals(item.Value, value);
	public bool ContainsKey(object key) => _memoryCache.TryGetValue(key, out _);
	public void CopyTo(KeyValuePair<object, object?>[] array, int _)
	{
		foreach (var tuple in array) { Add(tuple); }
	}

	public IEnumerator<KeyValuePair<object, object?>> GetEnumerator() => GetEnumerable().GetEnumerator();

	public bool Remove(object key)
	{
		var ok = _keys.Remove(key);
		_memoryCache.Remove(key);
		return ok;
	}

	bool ICollection<KeyValuePair<object, object?>>.Remove(KeyValuePair<object, object?> item) => ((IDictionary<object, object?>)this).Remove(item.Key);
	public bool TryGetValue(object key, [MaybeNullWhen(false)] out object? value) => ((IMemoryCache)this).TryGetValue(key, out value);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion idictionary implementation

	#region imemorycache implementation
	public ICacheEntry CreateEntry(object key)
	{
		if (!_keys.Contains(key)) { _keys.Add(key); }
		return _memoryCache.CreateEntry(key);
	}

	void IMemoryCache.Remove(object key) => Remove(key);
	bool IMemoryCache.TryGetValue(object key, out object? value) => _memoryCache.TryGetValue(key, out value);
	#endregion imemorycache implementation

	#region idisposable implementation
	private bool _disposed;

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				Clear();
				_memoryCache.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion idisposable implementation
}
