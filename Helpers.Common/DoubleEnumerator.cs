using Dawn;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Helpers.Common
{
	public class DoubleEnumerator<T> : IEnumerator<(T, T)>
	{
		private readonly IEnumerator<T> _first, _second;

		public DoubleEnumerator(IEnumerator<T> first, IEnumerator<T> second)
		{
			_first = Guard.Argument(() => first).NotNull().Value;
			_second = Guard.Argument(() => second).NotNull().Value;
		}

		public (T, T) Current => (_first.Current, _second.Current);

		object IEnumerator.Current => Current;

		public bool MoveNext() => _first.MoveNext() && _second.MoveNext();

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
}
