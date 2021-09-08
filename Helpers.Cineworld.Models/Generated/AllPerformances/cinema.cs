namespace Helpers.Cineworld.Models.Generated.AllPerformances;

public partial class cinema
{
	public static explicit operator Models.Cinema(cinema other)
		=> new(other.id, other.name, other.postcode);
}
