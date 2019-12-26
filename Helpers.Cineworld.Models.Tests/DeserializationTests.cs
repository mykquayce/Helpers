using Helpers.Cineworld.Models.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class DeserializationTests : IClassFixture<ExtensionMethodTestsFixture>
	{
		private readonly XmlSerializer _xmlSerializer;

		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};

		public DeserializationTests()
		{
			_xmlSerializer = new XmlSerializer(typeof(cinemasType));
		}

		[Theory]
		[InlineData("all-performances.xml")]
		public void DeserializationTests_Test1(string fileName)
		{
			// Act
			var cinemas = Deserialize(fileName);

			// Assert
			Assert.NotNull(cinemas);
			Assert.NotNull(cinemas.cinema);
			Assert.NotEmpty(cinemas.cinema);

			var titles = new List<string>();

			foreach (var cinema in cinemas.cinema)
			{
				Assert.NotNull(cinema.name);
				Assert.InRange(cinema.id, 1, short.MaxValue);
				Assert.NotNull(cinema.films);
				Assert.NotEmpty(cinema.films);

				foreach (var film in cinema.films)
				{
					Assert.Matches(@"^(\d{2,3}) mins$", film.length);

					Assert.InRange(film.Duration, 10d, 999d);
					Assert.Equal(film.length, $"{film.Duration:D} mins");

					Assert.NotNull(film.title);
					Assert.InRange(film.edi, 1, int.MaxValue);
					Assert.NotNull(film.shows);
					Assert.NotEmpty(film.shows);

					titles.Add(film.title);

					foreach (var show in film.shows)
					{
						Assert.NotNull(show.date);
						Assert.Matches(@"^(Sun|Mon|Tue|Wed|Thu|Fri|Sat) \d\d (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)$", show.date);
						Assert.NotNull(show.time);
						Assert.Matches(@"^\d\d:\d\d$", show.time);
					}
				}
			}

			var deduped = titles.GroupBy(s => s).Select(g => g.Key).ToList();
		}

		[Theory]
		[InlineData(
			@"{ ""cinemaIds"": [1, 2, 3], ""timesOfDay"": ""Afternoon|Evening"", ""daysOfWeek"": ""Friday|Saturday"", ""weekCount"": 1, ""titles"": [""jaws"", ""star wars""] }",
			new short[3] { 1, 2, 3, },
			TimesOfDay.Afternoon | TimesOfDay.Evening,
			DaysOfWeek.Friday | DaysOfWeek.Saturday,
			1,
			new string[2] { "jaws", "star wars", })]
		[InlineData(
			@"{ ""cinemaIds"": [4], ""titles"": [""unlimited"", ""preview""] }",
			new short[1] { 4, },
			TimesOfDay.AllDay,
			DaysOfWeek.AllWeek,
			byte.MaxValue,
			new string[2] { "unlimited", "preview", })]
		public void DeserializationTests_Queries(
			string json,
			IEnumerable<short> expectedCinemaIds, TimesOfDay expectedTimesOfDay, DaysOfWeek expectedDaysOfWeek, byte expectedWeekCount, IEnumerable<string> expectedTitles)
		{
			var actual = JsonSerializer.Deserialize<Query>(json, _jsonSerializerOptions);

			Assert.Equal(expectedCinemaIds, actual.CinemaIds);
			Assert.Equal(expectedTimesOfDay, actual.TimesOfDay);
			Assert.Equal(expectedDaysOfWeek, actual.DaysOfWeek);
			Assert.Equal(expectedWeekCount, actual.WeekCount);
			Assert.Equal(expectedTitles, actual.Titles);
		}

		private cinemasType Deserialize(string fileName)
		{
			var path = Path.Combine("Data", fileName);

			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			return (cinemasType)_xmlSerializer.Deserialize(stream);
		}
	}
}
