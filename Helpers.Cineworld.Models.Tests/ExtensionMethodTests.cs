using Helpers.Cineworld.Models.Enums;
using System;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class ExtensionMethodTests : IClassFixture<ExtensionMethodTestsFixture>
	{
		[Theory]
		[InlineData("", Formats.None, "")]
		[InlineData("(2D) Ad Astra", Formats._2d, "Ad Astra")]
		[InlineData("(2D) The Lion King", Formats._2d, "Lion King, The")]
		[InlineData("(4DX) War (Hindi)", Formats._2d | Formats._4dx, "War (Hindi)")]
		[InlineData("(IMAX 3-D) To The Arctic", Formats._3d | Formats.Imax, "To The Arctic")]
		[InlineData("(IMAX) Joker", Formats._2d | Formats.Imax, "Joker")]
		[InlineData("(IMAX) The Aeronauts Unlimited Screening", Formats._2d | Formats.Imax | Formats.SecretUnlimitedScreening, "Aeronauts, The")]
		[InlineData("(ScreenX) It: Chapter Two", Formats._2d | Formats.ScreenX, "It: Chapter Two")]
		[InlineData("(SS) Joker", Formats._2d | Formats.Subtitled, "Joker")]
		[InlineData("A Shaun The Sheep Movie: Farmageddon", Formats._2d, "Shaun The Sheep Movie: Farmageddon, A")]
		[InlineData("Asuran (Tamil)", Formats._2d, "Asuran (Tamil)")]
		[InlineData("Autism Friendly Screening: Dora And The Lost City", Formats._2d | Formats.AutismFriendlyScreening, "Dora And The Lost City")]
		[InlineData("Knives Out : Unlimited Screening", Formats._2d | Formats.SecretUnlimitedScreening, "Knives Out")]
		[InlineData("M4J Toy Story 4", Formats._2d | Formats.MoviesForJuniors, "Toy Story 4")]
		[InlineData("SubM4J Playmobil: The Movie", Formats._2d | Formats.MoviesForJuniors | Formats.Subtitled, "Playmobil: The Movie")]
		[InlineData("The Mustang", Formats._2d, "Mustang, The")]
		[InlineData("The Day Shall Come: Unlimited Screening", Formats._2d | Formats.SecretUnlimitedScreening, "Day Shall Come, The")]
		public void DeserializationTests_Formats(string title, Formats expectedFormats, string expectedTitle)
		{
			// Act
			var (actualTitle, actualFormats) = title.DeconstructTitle();
			actualTitle = actualTitle.DeArticlize();

			// Assert
			Assert.Equal(expectedFormats, actualFormats);
			Assert.Equal(expectedTitle, actualTitle);
		}

		[Theory]
		[InlineData(default, default)]
		[InlineData("", "")]
		[InlineData(" ", " ")]
		[InlineData("The", "The")]
		[InlineData("Ad Astra", "Ad Astra")]
		[InlineData("The Lion King", "Lion King, The")]
		[InlineData("A Shaun The Sheep Movie: Farmageddon", "Shaun The Sheep Movie: Farmageddon, A")]
		public void DeserializationTests_DeArticlize(string before, string expected)
		{
			Assert.Equal(expected, before.DeArticlize());
		}

		[Theory]
		[InlineData("Thu 10 Oct", 2019, 10, 10)]
		[InlineData("Thu 10 Nov", 2019, 11, 10)]
		public void ExtensionMethodTests_ParseDate(string before, int expectedYear, int expectedMonth, int expectedDay)
		{
			// Arrange
			ExtensionMethods.GetUtcNow = () => new DateTime(2019, 10, 24, 15, 23, 32, 547, DateTimeKind.Utc);

			// Act
			var actual = before.ParseDate();

			// Assert
			Assert.Equal(expectedYear, actual.Year);
			Assert.Equal(expectedMonth, actual.Month);
			Assert.Equal(expectedDay, actual.Day);
			Assert.Equal(0, actual.Hour);
			Assert.Equal(0, actual.Minute);
			Assert.Equal(0, actual.Second);
			Assert.Equal(0, actual.Millisecond);
			Assert.Equal(DateTimeKind.Utc, actual.Kind);
		}

		[Theory]
		[InlineData("10:30", 10, 30)]
		[InlineData("12:00", 12, 00)]
		[InlineData("23:15", 23, 15)]
		public void ExtensionMethodTests_ParseTime(string before, int expectedHours, int expectedMinutes)
		{
			// Act
			var actual = before.ParseTime();

			// Assert
			Assert.InRange(actual.Ticks, 0, TimeSpan.TicksPerDay);
			Assert.Equal(expectedHours, actual.Hours);
			Assert.Equal(expectedMinutes, actual.Minutes);
			Assert.Equal(0, actual.Seconds);
			Assert.Equal(0, actual.Milliseconds);
		}
	}
}
