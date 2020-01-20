using Helpers.Cineworld.Models.Enums;
using System;
using System.Linq;
using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class ExtensionMethodTests
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
		public void ExtensionMethodTests_Formats(string title, Formats expectedFormats, string expectedTitle)
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
		public void ExtensionMethodTests_DeArticlize(string before, string expected)
		{
			Assert.Equal(expected, before.DeArticlize());
		}

		[Theory]
		[InlineData("119 mins", 119)]
		[InlineData("1 mins", 1)]
		public void ExtensionMethodTests_ParseLength(string s, short expected)
		{
			var actual = s.ParseLength();

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(TimesOfDay.Night, 0)]
		[InlineData(TimesOfDay.Night, 1)]
		[InlineData(TimesOfDay.Night, 2)]
		[InlineData(TimesOfDay.Night, 3)]
		[InlineData(TimesOfDay.Night, 4)]
		[InlineData(TimesOfDay.Night, 5)]
		[InlineData(TimesOfDay.Morning, 6)]
		[InlineData(TimesOfDay.Morning, 7)]
		[InlineData(TimesOfDay.Morning, 8)]
		[InlineData(TimesOfDay.Morning, 9)]
		[InlineData(TimesOfDay.Morning, 10)]
		[InlineData(TimesOfDay.Morning, 11)]
		[InlineData(TimesOfDay.Afternoon, 12)]
		[InlineData(TimesOfDay.Afternoon, 13)]
		[InlineData(TimesOfDay.Afternoon, 14)]
		[InlineData(TimesOfDay.Afternoon, 15)]
		[InlineData(TimesOfDay.Afternoon, 16)]
		[InlineData(TimesOfDay.Afternoon, 17)]
		[InlineData(TimesOfDay.Evening, 18)]
		[InlineData(TimesOfDay.Evening, 19)]
		[InlineData(TimesOfDay.Evening, 20)]
		[InlineData(TimesOfDay.Evening, 21)]
		[InlineData(TimesOfDay.Evening, 22)]
		[InlineData(TimesOfDay.Evening, 23)]
		[InlineData(TimesOfDay.Night, 0, 1, 2, 3, 4, 5)]
		[InlineData(TimesOfDay.Morning, 6, 7, 8, 9, 10, 11)]
		[InlineData(TimesOfDay.Afternoon, 12, 13, 14, 15, 16, 17)]
		[InlineData(TimesOfDay.Evening, 18, 19, 20, 21, 22, 23)]
		[InlineData(TimesOfDay.AM, 5, 6)]
		[InlineData(TimesOfDay.Morning | TimesOfDay.Afternoon, 11, 12)]
		[InlineData(TimesOfDay.AllDay, 2, 8, 15, 20)]
		public void ExtensionMethodTests_ToTimesOfDay(TimesOfDay expected, params int[] hours)
		{
			var dateTimes = hours.Select(hour => new DateTime(2020, 1, 19, hour, 53, 27, DateTimeKind.Utc));

			var actual = dateTimes.ToTimesOfDay();

			Assert.Equal(expected, actual);
		}
	}
}
