namespace Helpers.Elgato.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd-party")]
public record AccessoryInfoObject(
	string productName,
	int hardwareBoardType,
	int firmwareBuildNumber,
	Version firmwareVersion,
	string serialNumber,
	string displayName,
	IReadOnlyCollection<string> features);
