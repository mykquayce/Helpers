namespace Helpers.Cineworld.Models;

public record Cinema(short Id, string Name, string Postcode)
{
	public static explicit operator Cinema(Generated.AllPerformances.cinema other)
		=> new(other.id, other.name, other.postcode.Trim());
}
