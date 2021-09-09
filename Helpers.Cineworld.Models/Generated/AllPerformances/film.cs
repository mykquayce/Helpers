namespace Helpers.Cineworld.Models.Generated.AllPerformances;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public partial class film
{
	public static explicit operator Models.Film(film other)
		=> new(other.edi, other.title, other.Length);

	public short Length => short.TryParse(this.length, out var result) ? result : default;
}
