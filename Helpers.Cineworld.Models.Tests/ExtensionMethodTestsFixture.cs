using System;

namespace Helpers.Cineworld.Models.Tests
{
	public sealed class ExtensionMethodTestsFixture : IDisposable
	{
		private readonly Func<DateTime> _getUtcNowFunc;

		public ExtensionMethodTestsFixture()
		{
			_getUtcNowFunc = ExtensionMethods.GetUtcNow;
			ExtensionMethods.GetUtcNow = () => new DateTime(2019, 10, 24, 15, 23, 32, 547, DateTimeKind.Utc);
		}

		public void Dispose()
		{
			ExtensionMethods.GetUtcNow = _getUtcNowFunc;
		}
	}
}
