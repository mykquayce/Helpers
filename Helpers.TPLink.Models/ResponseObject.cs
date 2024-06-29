namespace Helpers.TPLink.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public readonly record struct ResponseObject(ResponseObject.SystemObject system, ResponseObject.EmeterObject emeter)
{
	public record struct SystemObject(SystemInfoObject get_sysinfo);
	public record struct EmeterObject(RealtimeInfoObject get_realtime);
}
