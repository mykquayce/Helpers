namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record AllObject(
	IReadOnlyDictionary<string, LightObject> lights,
	IReadOnlyDictionary<string, GroupObject> groups,
	ConfigObject config,
	IReadOnlyDictionary<string, ScheduleObject> schedules,
	IReadOnlyDictionary<string, SceneObject> scenes,
	IReadOnlyDictionary<string, RuleObject> rules,
	IReadOnlyDictionary<string, SensorObject> sensors,
	IReadOnlyDictionary<string, ResourceLinkObject> resourceLink);
