using System;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class showTypeTests : IClassFixture<ExtensionMethodTestsFixture>
	{
		[Theory]
		[InlineData("Wed 23 Oct", "18:00", 2019, 10, 23, 17, 0)]
		[InlineData("Tue 12 Nov", "19:30", 2019, 11, 12, 19, 30)]
		[InlineData("Sat 28 Feb", "17:55", 2020, 2, 28, 17, 55)]
		[InlineData("Sat 29 Feb", "17:55", 2020, 2, 29, 17, 55)]
		public void showTypeTests_DateTime(
			string date, string time,
			int expectedYear, int expectedMonth, int expectedDay, int expectedHour, int expectedMinute)
		{
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
