using Xunit;

namespace Helpers.Cineworld.Models.Tests;

public class CastTests
{
	[Theory]
	[InlineData(1, "Cineworld Aberdeen - Queens Links", "AB24 5EN")]
	public void CastToRecord(short id, string name, string postcode)
	{
		var cinema = new Models.Generated.AllPerformances.cinema
		{
			id = id,
			name = name,
			postcode = postcode,
		};

		var actual = (Models.Cinema)cinema;

		Assert.Equal(actual.Id, id);
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

	[Theory]
	[InlineData("Fri 01 Oct", "11:00", 2021, 10, 1, 10, 0)]
	[InlineData("Mon 01 Nov", "11:00", 2021, 11, 1, 11, 0)]
	[InlineData("Sun 23 Jan", "11:00", 2022, 1, 23, 11, 0)]
	public void ParseDateTime(string date, string time, int year, int month, int day, int hour, int minute)
	{
		Generated.AllPerformances.show.GetTodayFunc = () => new DateTime(2021, 9, 30, 0, 0, 0, DateTimeKind.Utc);

		var expected = new DateTime(year, month, day, hour, minute, 0, 0, DateTimeKind.Utc);
		var show = new Helpers.Cineworld.Models.Generated.AllPerformances.show
		{
			date = date,
			time = time,
		};

		Assert.Equal(expected, show.DateTime);
	}

	[Theory]
	[InlineData("1 mins", 1)]
	[InlineData("10 mins", 10)]
	[InlineData("100 mins", 100)]
	[InlineData("1000 mins", 1_000)]
	public void ParseLength(string length, short expected)
	{
		var film = new Helpers.Cineworld.Models.Generated.AllPerformances.film
		{
			length = length,
		};

		Assert.Equal(expected, film.Length);
	}
}
