using Helpers.PhilipsHue.Models;

namespace Helpers.PhilipsHue.Clients;

public interface IPhilipsHueClient
{
	Task<AllObject> GetAllAsync();
	Task<ConfigObject> GetConfigAsync();
	Task<GroupObject> GetGroupAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, GroupObject>> GetGroupsAsync();
	Task<LightObject> GetLightAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, LightObject>> GetLightsAsync();
	Task<ResourceLinkObject> GetResourceLinkObjectAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, ResourceLinkObject>> GetResourceLinkObjectsAsync();
	Task<RuleObject> GetRuleAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, RuleObject>> GetRulesAsync();
	Task<SceneObject> GetSceneAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, SceneObject>> GetScenesAsync();
	Task<ScheduleObject> GetScheduleAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, ScheduleObject>> GetSchedulesAsync();
	Task<SensorObject> GetSensorAsync(string id);
	IAsyncEnumerable<KeyValuePair<string, SensorObject>> GetSensorsAsync();
	Task SetLightStateAsync(string id, LightObject.StateObject state);
}
