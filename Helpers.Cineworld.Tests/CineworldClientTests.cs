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
			var yesterday = DateTime.UtcNow.Date.AddDays(-1);

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
						Assert.InRange(dateTime, yesterday, DateTime.MaxValue);
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
			var actual = await _cineworldClient.GetListingsLastModifiedDateAsync();

			// Assert
			Assert.NotEqual(default, actual);
			Assert.Equal(DateTimeKind.Utc, actual.Kind);
			Assert.InRange(actual, now.AddDays(-1), now);
		}
	}
}
