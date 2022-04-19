namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record GroupObject(
	string name, IReadOnlyList<string> lights, IReadOnlyList<object> sensors, string type, GroupObject.StateObject state,
	bool recycle, string _class, GroupObject.ActionObject action)
{
	public record StateObject(bool all_on, bool any_on);

	public record ActionObject(
		bool on, int bri, int hue, int sat, string effect,
		IReadOnlyList<float> xy, int ct, string alert, string colormode);
}
