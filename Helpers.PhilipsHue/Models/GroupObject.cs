namespace Helpers.PhilipsHue.Models;

#pragma warning disable IDE1006 // Naming Styles
public record GroupObject(
	string? name, string[]? lights, object[]? sensors, string? type, GroupObject.StateObject? state,
	bool? recycle, string? _class, GroupObject.ActionObject? action)
{
	public record StateObject(bool? all_on, bool? any_on);

	public record ActionObject(
		bool? on, int? bri, int? hue, int? sat, string? effect,
		float[]? xy, int? ct, string? alert, string? colormode);
}
#pragma warning restore IDE1006 // Naming Styles
