namespace Helpers.Cineworld.Models;

public static class Extensions
{
	public static IEnumerable<Show> ToShows(this Generated.Listings.cinemas other)
	{
		return from c in other.cinema
			   from f in c.listing
			   from s in f.shows
			   let time = s.time.ToUniversalTime()
			   select new Show(c.id, f.edi, time);
	}
}
