namespace Helpers.Cineworld.Models.Generated.AllPerformances;

public partial class film
{
	public static explicit operator Models.Film(film other)
		=> new(other.edi, other.title, ParseLength(other.length));

	public static short ParseLength(string s)
	{
		var ss = s.Split(' ', count: 2);
		return short.Parse(ss[0]);
	}
}
