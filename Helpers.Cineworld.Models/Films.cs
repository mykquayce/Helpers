using System.Collections;

namespace Helpers.Cineworld.Models;

public record Films(IReadOnlyCollection<Film> Items) : IReadOnlyCollection<Film>
{
	#region IReadOnlyCollection implementation
	public int Count => Items.Count;
	public IEnumerator<Film> GetEnumerator() => Items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion IReadOnlyCollection implementation

	public static explicit operator Films(Generated.AllPerformances.cinemas other)
	{
		var query = from c in other.cinema
					from f in c.films
					select (Film)f;

		return new Films(query.ToList());
	}
}
