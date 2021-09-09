namespace Helpers.Cineworld.Models.Generated.AllPerformances;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public partial class cinema
{
	public static explicit operator Models.Cinema(cinema other)
		=> new(other.id, other.name, other.postcode);
}
