namespace Helpers.OpenWrt.Models;


[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record ResponseObject(int? id, string? result, ResponseObject.ErrorObject? error)
{
	public ResponseObject() : this(default, default, default) { }

	public record ErrorObject(string? message, int? code)
	{
		public ErrorObject() : this(default, default) { }
	}
}
