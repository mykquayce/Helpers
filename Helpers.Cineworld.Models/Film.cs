namespace Helpers.Cineworld.Models;

public record Film(int Edi, string Title, short Length)
{
	public static explicit operator Film(Generated.AllPerformances.film other)
	{
		var length = short.TryParse(other.length[..^5], out var result) ? result : default;
		return new(other.edi, other.title, length);
	}
}
