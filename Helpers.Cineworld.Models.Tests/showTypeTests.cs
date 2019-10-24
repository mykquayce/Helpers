using System;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class showTypeTests : IDisposable
	{
		private readonly Func<DateTime> _getUtcNow;

		public showTypeTests()
		{
			_getUtcNow = ExtensionMethods.GetUtcNow;
		}

		public void Dispose()
		{
			Dispose(managed: true);

			GC.SuppressFinalize(obj: this);
		}

		protected virtual void Dispose(bool managed)
		{
			ExtensionMethods.GetUtcNow = _getUtcNow;
		}

		[Theory]
		[InlineData("Wed 23 Oct", "18:00", 2019, 10, 23, 17, 0)]
		[InlineData("Tue 12 Nov", "19:30", 2019, 11, 12, 19, 30)]
		[InlineData("Sat 28 Feb", "17:55", 2020, 2, 28, 17, 55)]
		[InlineData("Sat 29 Feb", "17:55", 2020, 2, 29, 17, 55)]
		public void showTypeTests_DateTime(
			string date, string time,
			int expectedYear, int expectedMonth, int expectedDay, int expectedHour, int expectedMinute)
		{
			// Arrange
			ExtensionMethods.GetUtcNow = () => new DateTime(2019, 10, 24, 15, 23, 32, 547, DateTimeKind.Utc);

			// Act
			var show = new showType
			{
				date = date,
				time = time,
			};

			var actual = show.DateTime;

			// Assert
			Assert.Equal(expectedYear, actual.Year);
			Assert.Equal(expectedMonth, actual.Month);
			Assert.Equal(expectedDay, actual.Day);
			Assert.Equal(expectedHour, actual.Hour);
			Assert.Equal(expectedMinute, actual.Minute);
			Assert.Equal(0, actual.Second);
			Assert.Equal(0, actual.Millisecond);
			Assert.Equal(DateTimeKind.Utc, actual.Kind);
		}
	}
}
