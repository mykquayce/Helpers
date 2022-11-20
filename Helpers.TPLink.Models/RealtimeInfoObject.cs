namespace Helpers.TPLink.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record struct RealtimeInfoObject(int current_ma, int voltage_mv, int power_mw);
