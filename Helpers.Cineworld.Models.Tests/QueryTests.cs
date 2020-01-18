using System.Text.Json;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class QueryTests
	{
		[Fact]
		public void QueryTests_Deserialize_BehavesPredictably()
		{
			var json = @"{ ""CinemaIds"": [ 1 ], ""TimesOfDay"": ""PM"", ""DaysOfWeek"": ""Friday"", ""WeekCount"": 1, ""Titles"": [ ""preview"", ""unlimited"" ] }";

			var actual = JsonSerializer.Deserialize<Query>(json);

			Assert.NotNull(actual);
			Assert.NotNull(actual.CinemaIds);
			Assert.Single(actual.CinemaIds);
			Assert.Equal(1, actual.CinemaIds[0]);
			Assert.Equal(Enums.TimesOfDay.PM, actual.TimesOfDay);
			Assert.Equal(Enums.DaysOfWeek.Friday, actual.DaysOfWeek);
			Assert.Equal(1, actual.WeekCount);
			Assert.NotNull(actual.Titles);
			Assert.NotEmpty(actual.Titles);
			Assert.Equal(2, actual.Titles.Count);
			Assert.Equal("preview", actual.Titles[0]);
			Assert.Equal("unlimited", actual.Titles[1]);
		}
	}
}
