using System;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Cineworld.Tests
{
	public sealed class CineworldClientTests : IDisposable
	{
		private readonly ICineworldClient _cineworldClient;

		public CineworldClientTests()
		{
			_cineworldClient = new Concrete.CineworldClient();
		}

		public void Dispose()
		{
			_cineworldClient?.Dispose();
		}

		[Fact]
		public async Task CineworldClientTests_GetPerformancesAsync_ReturnsValidXml()
		{
			// Arrange
			var today = DateTime.UtcNow.Date;

			// Act
			var cinemas = await _cineworldClient.GetPerformancesAsync();

			// Assert
			Assert.NotNull(cinemas);
			Assert.NotNull(cinemas.cinema);
			Assert.NotEmpty(cinemas.cinema);

			foreach (var cinema in cinemas.cinema)
			{
				Assert.NotNull(cinema.name);
				Assert.InRange(cinema.id, 1, short.MaxValue);
				Assert.NotNull(cinema.films);
				Assert.NotEmpty(cinema.films);

				foreach (var film in cinema.films)
				{
					Assert.Matches(@"^\d{2,3} mins$", film.length);

					Assert.InRange(film.Duration, 10, 999);
					Assert.Equal(film.length, $"{film.Duration:D} mins");

					Assert.NotNull(film.title);
					Assert.InRange(film.edi, 0, int.MaxValue);
					Assert.NotNull(film.shows);
					Assert.NotEmpty(film.shows);

					foreach (var show in film.shows)
					{
						Assert.NotNull(show.date);
						Assert.Matches(@"^(Sun|Mon|Tue|Wed|Thu|Fri|Sat) \d\d (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)$", show.date);
						Assert.NotNull(show.time);
						Assert.Matches(@"^\d\d:\d\d$", show.time);

						Assert.InRange(show.DateTime, today, DateTime.MaxValue);
					}
				}
			}
		}

		[Fact]
		public async Task CineworldClientTests_GetPerformancesLastModifiedDateAsync_ReturnsUtcDateTime()
		{
			// Arrange
			var now = DateTime.UtcNow;

			// Act
			var actual = await _cineworldClient.GetPerformancesLastModifiedDateAsync();

			// Assert
			Assert.NotEqual(default, actual);
			Assert.Equal(DateTimeKind.Utc, actual.Kind);
			Assert.InRange(actual, now.AddDays(-1), now);
		}
	}
}
