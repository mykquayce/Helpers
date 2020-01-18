using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class FilmTypeTests
	{
		[Theory]
		[InlineData("1917", "1917", Enums.Formats._2d)]
		[InlineData("M4J A Shaun The Sheep Movie: Farmageddon", "Shaun The Sheep Movie: Farmageddon, A", Enums.Formats._2d | Enums.Formats.MoviesForJuniors)]
		[InlineData("Bombshell : Unlimited Screening", "Bombshell", Enums.Formats._2d | Enums.Formats.SecretUnlimitedScreening)]
		public void FilmTypeTests_Formats(string title, string expectedTitle, Enums.Formats expectedFormats)
		{
			// Arrange, Act
			var film = new Generated.FilmType { Title = title, };

			// Assert
			Assert.Equal(expectedTitle, film.Title);
			Assert.Equal(expectedFormats, film.Formats);
		}
	}
}
