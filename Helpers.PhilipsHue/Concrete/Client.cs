using Dawn;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Helpers.PhilipsHue.Concrete;

public class Client : Helpers.Web.WebClientBase, IClient
{
	private readonly string _username;

	public Client(HttpClient httpClient, IOptions<Config> config)
		: base(httpClient)
	{
		Guard.Argument(httpClient).NotNull().Wrap(c => c.BaseAddress)
			.NotNull().Wrap(uri => uri.OriginalString)
			.NotNull().NotEmpty().NotWhiteSpace();

		_username = Guard.Argument(config).NotNull().Wrap(o => o.Value)
			.NotNull().Wrap(c => c.Username)
			.NotNull().NotEmpty().NotWhiteSpace().Matches("^[0-9A-Za-z]{40}$")
			.Value;
	}

	private async Task<T> GetAsync<T>(string uriSuffix)
		where T : class
	{
		var uri = new Uri($"/api/{_username}/{uriSuffix}", UriKind.Relative);
		(_, _, var t) = await base.SendAsync<T>(HttpMethod.Get, uri);
		return t;
	}

	private async IAsyncEnumerable<KeyValuePair<TKey, TValue>> GetAsync<TKey, TValue>(string uriSuffix)
		where TKey : notnull
	{
		var uri = new Uri($"/api/{_username}/{uriSuffix}", UriKind.Relative);
		(_, _, var dictionary) = await base.SendAsync<IDictionary<TKey, TValue>>(HttpMethod.Get, uri);
		foreach (var kvp in dictionary) yield return kvp;
	}

	private async Task<T> PutAsync<T>(string uriSuffix, object body)
		where T : class
	{
		var uri = new Uri($"/api/{_username}/{uriSuffix}", UriKind.Relative);
		var json = JsonSerializer.Serialize(body);
		(_, _, var t) = await base.SendAsync<T>(HttpMethod.Put, uri, json);
		return t;
	}

	#region all
	public Task<Models.AllObject> GetAllAsync() => GetAsync<Models.AllObject>(string.Empty);
	#endregion all

	#region lights
	public Task<Models.LightObject> GetLightAsync(string id) => GetAsync<Models.LightObject>("lights/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.LightObject>> GetLightsAsync() => GetAsync<string, Models.LightObject>("lights");

	public Task SetLightStateAsync(string id, Models.LightObject.StateObject state) => PutAsync<string>($"lights/{id}/state", state);
	#endregion lights

	#region groups
	public Task<Models.GroupObject> GetGroupAsync(string id) => GetAsync<Models.GroupObject>("groups/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.GroupObject>> GetGroupsAsync() => GetAsync<string, Models.GroupObject>("groups");
	#endregion groups

	#region config
	public Task<Models.ConfigObject> GetConfigAsync() => GetAsync<Models.ConfigObject>("config");
	#endregion config

	#region schedules
	public Task<Models.ScheduleObject> GetScheduleAsync(string id) => GetAsync<Models.ScheduleObject>("schedules/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.ScheduleObject>> GetSchedulesAsync() => GetAsync<string, Models.ScheduleObject>("schedules");
	#endregion schedules

	#region scenes
	public Task<Models.SceneObject> GetSceneAsync(string id) => GetAsync<Models.SceneObject>("scenes/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.SceneObject>> GetScenesAsync() => GetAsync<string, Models.SceneObject>("scenes");
	#endregion scenes

	#region rules
	public Task<Models.RuleObject> GetRuleAsync(string id) => GetAsync<Models.RuleObject>("rules/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.RuleObject>> GetRulesAsync() => GetAsync<string, Models.RuleObject>("rules");
	#endregion rules

	#region sensor
	public Task<Models.SensorObject> GetSensorAsync(string id) => GetAsync<Models.SensorObject>("sensor/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.SensorObject>> GetSensorsAsync() => GetAsync<string, Models.SensorObject>("sensor");
	#endregion sensor

	#region resourcelinks
	public Task<Models.ResourceLinkObject> GetResourceLinkObjectAsync(string id) => GetAsync<Models.ResourceLinkObject>("resourcelinks/" + id);
	public IAsyncEnumerable<KeyValuePair<string, Models.ResourceLinkObject>> GetResourceLinkObjectsAsync() => GetAsync<string, Models.ResourceLinkObject>("resourcelinks");
	#endregion resourcelinks
}
