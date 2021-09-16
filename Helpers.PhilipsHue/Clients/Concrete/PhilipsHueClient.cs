using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.PhilipsHue.Clients.Concrete
{
	public class PhilipsHueClient : Helpers.Web.WebClientBase, IPhilipsHueClient
	{
		public record Config(string? BridgePhysicalAddress, string? Username)
		{
			// needed for deserialization
			public Config() : this(default, default) { }
		}

		private readonly string _username;

		public PhilipsHueClient(HttpClient httpClient, IOptions<Config> config)
			: this(httpClient, config.Value)
		{ }

		public PhilipsHueClient(HttpClient httpClient, Config config)
			: this(httpClient, config?.Username)
		{ }

		public PhilipsHueClient(HttpClient httpClient, string? username)
			: base(httpClient)
		{
			_username = Guard.Argument(username!)
				.NotNull()
				.NotEmpty()
				.NotWhiteSpace()
				.Matches(@"^[0-9A-Za-z]{40}$")
				.Value;
		}

		private async Task<T> SendAsync<T>(HttpMethod method, string uriSuffix, object? body = default) where T : class
		{
			var uri = new Uri($"/api/{_username}/{uriSuffix}", UriKind.Relative);
			string? json = body is not null ? JsonSerializer.Serialize(body) : default;
			var response = await base.SendAsync<T>(method, uri, json);
			return response.Object!;
		}

		private async IAsyncEnumerable<KeyValuePair<TKey, TValue>> SendAsync<TKey, TValue>(HttpMethod method, string uriSuffix, object? body = default)
		{
			var dictionary = await SendAsync<IDictionary<TKey, TValue>>(method, uriSuffix, body);
			foreach (var kvp in dictionary!) yield return kvp;
		}

		private Task<T> GetAsync<T>(string uriSuffix) where T : class => SendAsync<T>(HttpMethod.Get, uriSuffix);
		private Task<T> PutAsync<T>(string uriSuffix, object body) where T : class => SendAsync<T>(HttpMethod.Put, uriSuffix, body);
		private IAsyncEnumerable<KeyValuePair<TKey, TValue>> GetAsync<TKey, TValue>(string uriSuffix) => SendAsync<TKey, TValue>(HttpMethod.Get, uriSuffix);

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
}
