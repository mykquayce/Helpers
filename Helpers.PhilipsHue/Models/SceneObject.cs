namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record SceneObject(
	string name, string type, string group, IReadOnlyList<string> lights, string owner, bool recycle,
	bool locked, SceneObject.AppdataObject appdata, string picture, DateTime lastupdated,
	int version, IReadOnlyDictionary<string, SceneObject.LightStateObject> lightstates)
{
	public record AppdataObject(int version, string data);
	public record LightStateObject(bool on, byte bri, IReadOnlyList<float> xy);
}
