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
			await foreach(var cinema in _cineworldClient.GetListingsAsync())
			{
				// Assert
				Assert.NotNull(cinema.name);
				Assert.InRange(cinema.id, 1, short.MaxValue);
				Assert.NotNull(cinema.listing);
				Assert.NotEmpty(cinema.listing);

				foreach (var film in cinema.listing)
				{
					Assert.NotNull(film.title);
					Assert.InRange(film.edi, 0, int.MaxValue);
					Assert.NotNull(film.shows);
					Assert.NotEmpty(film.shows);

					foreach (var show in film.shows)
					{
						Assert.NotEqual(default, show.time);
						Assert.InRange(show.time, yesterday, DateTime.MaxValue);
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
