namespace Helpers.OpenWrt.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record RequestObject(int? id, string? method, params string[]? @params)
	{
		public RequestObject() : this(default, default, default) { }
	}
#pragma warning restore IDE1006 // Naming Styles
}
