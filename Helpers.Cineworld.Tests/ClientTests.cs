using Xunit;

namespace Helpers.Cineworld.Tests;

public class ClientTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly IClient _sut;
	public ClientTests(Fixtures.ClientFixture fixture)
	{
		_sut = fixture.Client;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "calls 3rd party")]
	[Fact(Skip = "calls 3rd party api")]
	public async Task GetAllPerformances()
	{
		var cinemas = await _sut.GetAllPerformancesAsync();

		Assert.NotNull(cinemas);
		Assert.NotNull(cinemas.cinema);
		Assert.NotEmpty(cinemas.cinema);

		foreach (var cinema in cinemas.cinema)
		{
			Assert.NotNull(cinema);
			Assert.NotEqual(0, cinema.id);
			Assert.NotNull(cinema.name);
			Assert.NotEmpty(cinema.name);
			Assert.NotNull(cinema.postcode);
			Assert.NotEmpty(cinema.postcode);
			Assert.NotNull(cinema.films);
			Assert.NotEmpty(cinema.films);

			foreach (var film in cinema.films)
			{
				Assert.NotNull(film);
				Assert.NotEqual(0, film.edi);
				Assert.NotNull(film.title);
				Assert.NotEmpty(film.title);
				Assert.NotNull(film.length);
				Assert.Matches(@"\d+ mins", film.length);
			}
		}
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "calls 3rd party")]
	[Fact(Skip = "calls 3rd party api")]
	public async Task GetListings()
	{
		var cinemas = await _sut.GetListingsAsync();

		Assert.NotNull(cinemas);
		Assert.NotNull(cinemas.cinema);
		Assert.NotEmpty(cinemas.cinema);

		foreach (var cinema in cinemas.cinema)
		{
			Assert.NotNull(cinema);
			Assert.NotEqual(0, cinema.id);
			Assert.NotNull(cinema.listing);
			Assert.NotEmpty(cinema.listing);

			foreach (var film in cinema.listing)
			{
				Assert.NotNull(film);
				Assert.NotEqual(0, film.edi);
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
