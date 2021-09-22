using Xunit;

namespace Helpers.Cineworld.Models.Tests;

public class ListingsTests
{
	[Theory]
	[InlineData(".", "Data", "listings.xml")]
	public async Task Listings(params string[] paths)
	{
		var path = Path.Combine(paths);
		await using var stream = File.OpenRead(path);

		var cinemas = stream.Deserialize<Helpers.Cineworld.Models.Generated.Listings.cinemas>();

		Assert.NotNull(cinemas);
		Assert.NotNull(cinemas!.cinema);
		Assert.NotEmpty(cinemas.cinema);

		foreach (var cinema in cinemas.cinema)
		{
			Assert.NotNull(cinema);
			Assert.NotNull(cinema.listing);
			Assert.NotEmpty(cinema.listing);

			foreach (var film in cinema.listing)
			{
				Assert.NotNull(film);
				Assert.NotNull(film.shows);
				Assert.NotEmpty(film.shows);

				foreach (var show in film.shows)
				{
					Assert.NotNull(show);
					Assert.NotEqual(default, show.time);
				}
			}
		}
	}
}
