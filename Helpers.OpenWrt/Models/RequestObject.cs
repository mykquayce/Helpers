namespace Helpers.OpenWrt.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record RequestObject(int? id, string? method, params string[]? @params)
{
	public RequestObject() : this(default, default, default) { }
}
