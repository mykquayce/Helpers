using System.Collections.Generic;

namespace Helpers.PhilipsHue.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record AllObject(
		IDictionary<string, LightObject>? lights,
		IDictionary<string, GroupObject>? groups,
		ConfigObject? config,
		IDictionary<string, ScheduleObject>? schedules,
		IDictionary<string, SceneObject>? scenes,
		IDictionary<string, RuleObject>? rules,
		IDictionary<string, SensorObject>? sensors,
		IDictionary<string, ResourceLinkObject>? resourceLink);
#pragma warning restore IDE1006 // Naming Styles
}
