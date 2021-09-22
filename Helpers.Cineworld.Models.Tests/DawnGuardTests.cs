using Dawn;
using Xunit;

namespace Helpers.Cineworld.Models.Tests;

public class DawnGuardTests
{
	[Theory]
	[InlineData(".", "Data", "all-performances.xml")]
	public async Task AllPerformances(params string[] paths)
	{
		var path = Path.Combine(paths);
		await using var stream = File.OpenRead(path);

		var cinemas = stream.Deserialize<Helpers.Cineworld.Models.Generated.AllPerformances.cinemas>();

		foreach (var cinema in cinemas!.cinema)
		{
			var c = (Models.Cinema)cinema;
			DawnGuardExtensions.IsCinema(Guard.Argument(c));

			foreach (var film in cinema.films)
			{
				var f = (Models.Film)film;
				DawnGuardExtensions.IsFilm(Guard.Argument(f));
			}
		}
	}
}
