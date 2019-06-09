using System;

namespace Helpers.Tracing.Middleware
{
	public class Swap<T> : IDisposable
	{
		private readonly Action<T> _setter;
		private readonly T _original;

		public Swap(Func<T> getter, Action<T> setter, T replacement)
		{
			_original = getter();
			setter(replacement);
			_setter = setter;
		}

		public void Dispose()
		{
			_setter(_original);
		}
	}
}
