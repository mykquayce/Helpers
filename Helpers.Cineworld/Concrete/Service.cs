using Dawn;
using Helpers.Cineworld.Models;

namespace Helpers.Cineworld.Concrete;

public class Service : IService
{
	private readonly IClient _client;

	public Service(IClient client)
	{
		_client = Guard.Argument(() => client).NotNull().Value;
	}

	public async IAsyncEnumerable<Models.Cinema> GetCinemasAsync()
	{
		var cinemas = await _client.GetAllPerformancesAsync();

		foreach (var cinema in cinemas.cinema)
		{
			yield return (Models.Cinema)cinema;
		}
	}

	public async IAsyncEnumerable<Models.Film> GetFilmsAsync()
	{
		var cinemas = await _client.GetAllPerformancesAsync();

		var films = (
			from c in cinemas.cinema
			from f in c.films
			select (Models.Film)f
			).Distinct();

		foreach (var film in films)
		{
			yield return film;
		}
	}

	public async IAsyncEnumerator<Models.Show> GetShowsAsync()
	{
		var cinemas = await _client.GetListingsAsync();

		var shows = cinemas.ToShows();

		foreach (var show in shows)
		{
			yield return show;
		}
	}
}
