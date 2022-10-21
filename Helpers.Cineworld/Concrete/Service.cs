using Dawn;
using System.Runtime.CompilerServices;

namespace Helpers.Cineworld.Concrete;

public class Service : IService
{
	private readonly IClient _client;

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	public async IAsyncEnumerable<Models.Cinema> GetCinemasAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var cinemas = await _client.GetAllPerformancesAsync(cancellationToken);

		var enumerator = cinemas.cinema.GetEnumerator();
		while (!cancellationToken.IsCancellationRequested && enumerator.MoveNext())
		{
			var cinema = (Models.Generated.AllPerformances.cinema)enumerator.Current;
			yield return (Models.Cinema)cinema;
		}
	}

	public async IAsyncEnumerable<Models.Film> GetFilmsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var cinemas = await _client.GetAllPerformancesAsync(cancellationToken);

		var films = (
			from c in cinemas.cinema
			from f in c.films
			select (Models.Film)f
			).Distinct();

		var enumerator = films.GetEnumerator();
		while (!cancellationToken.IsCancellationRequested && enumerator.MoveNext())
		{
			var film = enumerator.Current;
			yield return enumerator.Current;
		}
	}

	public async IAsyncEnumerable<Models.Show> GetShowsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var cinemas = await _client.GetListingsAsync(cancellationToken);

		var shows = from c in cinemas.cinema
					from f in c.listing
					from s in f.shows
					let time = s.time.ToUniversalTime()
					select new Models.Show(c.id, f.edi, time);

		var enumerator = shows.GetEnumerator();
		while (!cancellationToken.IsCancellationRequested && enumerator.MoveNext())
		{
			yield return enumerator.Current;
		}
	}
}
