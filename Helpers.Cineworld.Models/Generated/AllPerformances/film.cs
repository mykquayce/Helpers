namespace Helpers.Cineworld.Models.Generated.AllPerformances;

public partial class film
{
	public short Length => short.Parse(this.length[..^5]);
}
