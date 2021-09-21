using System.Collections;

namespace Helpers.Cineworld.Models;

public record Shows(IReadOnlyCollection<Show> Items) : IReadOnlyCollection<Show>
{
	#region IReadOnlyCollection implenmentation
	public int Count => Items.Count;
	public IEnumerator<Show> GetEnumerator() => Items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion IReadOnlyCollection implenmentation

	public static explicit operator Shows(Generated.Listings.cinemas other)
	{
		var query = from c in other.cinema
					from f in c.listing
					from s in f.shows
					let time = s.time.ToUniversalTime()
					select new Show(c.id, f.edi, time);

		return new Shows(query.ToList());
	}
}
