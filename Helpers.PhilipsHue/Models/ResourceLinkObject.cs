namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record ResourceLinkObject(
	string name, string description, string type,
	int classid, string owner, bool recycle, IReadOnlyList<string> links);
