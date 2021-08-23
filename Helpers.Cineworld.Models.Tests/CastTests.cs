using Xunit;

namespace Helpers.Cineworld.Models.Tests;

public class CastTests
{
	[Theory]
	[InlineData(1, "Cineworld Aberdeen - Queens Links", "AB24 5EN")]
	public void CastToRecord(ushort id, string name, string postcode)
	{
		var cinema = new Models.Generated.AllPerformances.cinema
		{
			id = (short)id,
			name = name,
			postcode = postcode,
		};

		var actual = (Models.Cinema)cinema;

		Assert.Equal((int)actual.Id, (int)id);
		Assert.Equal(actual.Name, name);
		Assert.Equal(actual.Postcode, postcode);
	}

	[Theory]
	[InlineData(250_274, "Shang-Chi And The Legend Of The Ten Rings", "130 mins", 130)]
	public void ParseFilmLength(int edi, string title, string length, short expected)
	{
		var film = new Models.Generated.AllPerformances.film
		{
			edi = edi,
			title = title,
			length = length,
		};

		var actual = (Models.Film)film;

		Assert.Equal(edi, actual.Edi);
		Assert.Equal(title, actual.Title);
		Assert.Equal(expected, actual.Length);
	}
}
