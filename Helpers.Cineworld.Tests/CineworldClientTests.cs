using System;
using System.Linq;
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
		public async Task CineworldClientTests_GetListingsAsync_ReturnsValidXml()
		{
			// Arrange
			var today = DateTime.UtcNow.Date;
			var yesterday = today.AddDays(-1);
			var nextYear = today.AddYears(1);

			// Act
			await foreach (var cinema in _cineworldClient.GetListingsAsync())
			{
				// Assert
				Assert.NotNull(cinema.Name);
				Assert.InRange(cinema.Id, 1, short.MaxValue);
				Assert.NotNull(cinema.Films);
				Assert.NotEmpty(cinema.Films);

				foreach (var film in cinema.Films)
				{
					Assert.NotNull(film.Title);
					Assert.InRange(film.Edi, 0, int.MaxValue);
					Assert.NotNull(film.DateTimes);
					Assert.NotEmpty(film.DateTimes);

					foreach (var dateTime in film.DateTimes)
					{
						Assert.NotEqual(default, dateTime);
						Assert.InRange(dateTime, yesterday, nextYear);
					}
				}
			}
		}

		[Fact]
		public async Task CineworldClientTests_GeLastModifiedDateAsync_ReturnsUtcDateTime()
		{
			// Arrange
			var now = DateTime.UtcNow;

			// Act
			var actual = await _cineworldClient.GetLastModifiedDateAsync();

			// Assert
			Assert.NotEqual(default, actual);
			Assert.Equal(DateTimeKind.Utc, actual.Kind);
			Assert.InRange(actual, now.AddDays(-1), now);
		}

		[Fact]
		public async Task CineworldClientTests_GetFilmsAsync()
		{
			var films = await _cineworldClient.GetFilmsAsync().ToListAsync();

			Assert.NotNull(films);
			Assert.NotEmpty(films);

			foreach (var film in films)
			{
				Assert.NotNull(film);
				Assert.InRange(film.Edi, 0, int.MaxValue);
				Assert.NotNull(film.Title);
				Assert.NotEmpty(film.Title);
			}
		}
	}
}
