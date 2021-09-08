using Xunit;

namespace Helpers.Cineworld.Models.Tests;

public class AllPerformancesTests
{
	[Theory]
	[InlineData(".", "Data", "all-performances.xml")]
	public async Task AllPerformances(params string[] paths)
	{
		var path = Path.Combine(paths);
		await using var stream = File.OpenRead(path);

		var cinemas = stream.Deserialize<Helpers.Cineworld.Models.Generated.AllPerformances.cinemas>();

		Assert.NotNull(cinemas);
		Assert.NotNull(cinemas!.cinema);
		Assert.NotEmpty(cinemas.cinema);

		foreach (var cinema in cinemas.cinema)
		{
			Assert.NotNull(cinema);
			Assert.InRange(cinema!.id, 1, short.MaxValue);
			Assert.NotNull(cinema.name);
			Assert.NotEmpty(cinema.name);
			Assert.NotNull(cinema.postcode);
			Assert.NotEmpty(cinema.postcode);
			Assert.NotNull(cinema.films);

			foreach (var film in cinema.films)
			{
				Assert.NotNull(film);
				Assert.InRange(film.edi, 1, int.MaxValue);
				Assert.NotNull(film.title);
				Assert.NotEmpty(film.title);
				Assert.NotNull(film.length);
				Assert.Matches(@"^\d+ mins$", film.length);
			}
		}
	}
}
