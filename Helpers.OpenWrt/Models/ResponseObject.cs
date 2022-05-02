namespace Helpers.OpenWrt.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record ResponseObject(int id, string result, ResponseObject.ErrorObject? error)
{
	public record ErrorObject(string message, string data, int code);
}
