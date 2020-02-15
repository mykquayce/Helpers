using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class FilmTypeTests
	{
		[Theory]
		[InlineData("1917", "1917", Enums.Formats._2d)]
		[InlineData("M4J A Shaun The Sheep Movie: Farmageddon", "Shaun The Sheep Movie: Farmageddon, A", Enums.Formats._2d | Enums.Formats.MoviesForJuniors)]
		[InlineData("Bombshell : Unlimited Screening", "Bombshell", Enums.Formats._2d | Enums.Formats.SecretUnlimitedScreening)]
		[InlineData("Dementia Friendly Screening Calamity Jane", "Calamity Jane", Enums.Formats._2d | Enums.Formats.DementiaFriendlyScreening)]
		public void FilmTypeTests_Formats(string title, string expectedTitle, Enums.Formats expectedFormats)
		{
			// Arrange, Act
			var film = new Generated.FilmType { Title = title, };

			// Assert
			Assert.Equal(expectedTitle, film.Title);
			Assert.Equal(expectedFormats, film.Formats);
		}

		[Theory]
		[InlineData("Films.xml")]
		public void FilmTypeTests_Deserialize(string fileName)
		{
			var films = fileName.DeserializeFile<Models.FilmsType>();

			Assert.NotNull(films);
			Assert.NotNull(films.Film);
			Assert.NotEmpty(films.Film);

			foreach (var film in films.Film)
			{
				Assert.NotNull(film);
				Assert.InRange(film.Edi, 0, int.MaxValue);
				Assert.NotNull(film.Title);
				Assert.NotEmpty(film.Title);
			}
		}

		[Theory]
		[InlineData("(2D) 1917", "1917")]
		[InlineData("(3D) 1917", "1917 (_3d)")]
		[InlineData("(IMAX) 1917", "1917 (Imax)")]
		[InlineData("1917", "1917")]
		[InlineData("Dementia Friendly Screening Calamity Jane", "Calamity Jane (DementiaFriendlyScreening)")]
		public void FilmTypeTests_ToString(string title, string expected)
		{
			// Arrange
			var film = new Generated.FilmType { Title = title, };

			// Act
			var actual = film.ToString();

			// Assert
			Assert.Equal(expected, actual);
		}
	}
}
