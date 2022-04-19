using Helpers.PhilipsHue.Models;

namespace Helpers.PhilipsHue.Services;

public interface IPhilipsHueService
{
	IAsyncEnumerable<KeyValuePair<string, GroupObject>> GetGroupsByNamesAsync(params string[] names);
	IAsyncEnumerable<KeyValuePair<string, LightObject>> GetLightsByNamesAsync(params string[] names);
	Task SetGroupsStateAsync(LightObject.StateObject state, params string[] names);
	Task SetLightsStateAsync(LightObject.StateObject state, params string[] names);
	Task SetLightStateAsync(string groupName, string lightName, LightObject.StateObject state);
	Task ToggleLightsAsync(params string[] names);
	Task ToggleLightsInGroupsAsync(params string[] groupNames);
}
