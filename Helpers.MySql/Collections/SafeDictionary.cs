namespace Helpers.MySql.Collections;

public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
	private readonly TValue _default;

	public SafeDictionary(TValue @default)
	{
		_default = @default;
	}

	public new TValue this[TKey key]
	{
		get
		{
			return TryGetValue(key, out var result)
				? result
				: _default;
		}
		set
		{
			if (ContainsKey(key))
			{
				base[key] = value;
			}
			else
			{
				Add(key, value);
			}
		}
	}
}
