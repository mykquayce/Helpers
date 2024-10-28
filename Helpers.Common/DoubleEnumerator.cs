namespace System.Collections.Generic;

public class DoubleEnumerator<TFirst, TSecond> : IEnumerator<(TFirst, TSecond)>
{
	private readonly IEnumerator<TFirst> _first;
	private readonly IEnumerator<TSecond> _second;

	public DoubleEnumerator(IEnumerator<TFirst> first, IEnumerator<TSecond> second)
	{
		ArgumentNullException.ThrowIfNull(first);
		ArgumentNullException.ThrowIfNull(second);
		_first = first;
		_second = second;
	}

	public (TFirst, TSecond) Current => (_first.Current, _second.Current);

	object IEnumerator.Current => Current;

	public bool MoveNext()
	{
		var ok = new bool[2] { _first.MoveNext(), _second.MoveNext(), };
		return ok[0] && ok[1];
	}

	public void Reset()
	{
		_first.Reset();
		_second.Reset();
	}

	#region dispose
	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_first.Dispose();
				_second.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion dispose
}
