using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class CinemasTypeTests
	{
		[Theory]
		[InlineData("Cinemas.xml")]
		public void CinemasTypeTests_Deserialize(string fileName)
		{
			var cinemas = fileName.DeserializeFile<Models.Generated.CinemasType>();

			Assert.NotNull(cinemas);
			Assert.NotNull(cinemas.Cinema);
			Assert.NotEmpty(cinemas.Cinema);

			foreach (var cinema in cinemas.Cinema)
			{
				Assert.NotNull(cinema);
				Assert.InRange(cinema.Id, 1, short.MaxValue);
				Assert.NotNull(cinema.Name);
				Assert.NotEmpty(cinema.Name);
				Assert.NotNull(cinema.Films);
				Assert.NotEmpty(cinema.Films);

				foreach (var film in cinema.Films)
				{
					Assert.NotNull(film);
					Assert.InRange(film.Edi, 0, int.MaxValue);
					Assert.NotNull(film.Title);
					Assert.NotEmpty(film.Title);
					Assert.False(film.Title.StartsWith("A "));
					Assert.False(film.Title.StartsWith("An "));
					Assert.False(film.Title.StartsWith("The "));
					Assert.NotEqual(default, film.Formats);
					Assert.NotNull(film.DateTimes);
					Assert.NotEmpty(film.DateTimes);

					Assert.All(film.DateTimes, dt => Assert.NotEqual(default, dt));
				}
			}
		}
	}
}
