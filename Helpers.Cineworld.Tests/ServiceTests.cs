using Xunit;

namespace Helpers.Cineworld.Tests;

public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _sut;

	public ServiceTests(Fixtures.ServiceFixture fixture)
	{
		_sut = fixture.Service;
	}

	[Fact]
	public async Task GetCinemas()
	{
		var cinemas = await _sut.GetCinemasAsync().ToListAsync();

		Assert.NotNull(cinemas);
		Assert.NotEmpty(cinemas);

		foreach (var cinema in cinemas)
		{
			Assert.NotNull(cinema);
			Assert.NotEqual(0, cinema.Id);
			Assert.NotNull(cinema.Name);
			Assert.NotEmpty(cinema.Name);
			Assert.NotNull(cinema.Postcode);
			Assert.NotEmpty(cinema.Postcode);
		}
	}

	[Fact]
	public async Task GetFilms()
	{
		var films = await _sut.GetFilmsAsync().ToListAsync();

		Assert.NotNull(films);
		Assert.NotEmpty(films);

		foreach (var film in films)
		{
			Assert.NotNull(film);
			Assert.InRange(film.Edi, 0, int.MaxValue);
			Assert.NotNull(film.Title);
			Assert.NotEmpty(film.Title);
			Assert.InRange(film.Length, 1, short.MaxValue);
		}
	}

	[Fact]
	public async Task GetShows()
	{
		var shows = await _sut.GetShowsAsync().ToListAsync();

		Assert.NotNull(shows);
		Assert.NotEmpty(shows);

		foreach (var show in shows)
		{
			Assert.NotNull(show);
			Assert.InRange(show.CinemaId, 0, short.MaxValue);
			Assert.InRange(show.FilmEdi, 0, int.MaxValue);
			Assert.NotEqual(default, show.DateTime);
		}
	}
}
