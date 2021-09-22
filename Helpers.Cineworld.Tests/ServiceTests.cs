using Dawn;
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
			Guard.Argument(cinema).IsCinema();
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
			Guard.Argument(film).IsFilm();
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
			Guard.Argument(show).IsShow();
		}
	}
}
