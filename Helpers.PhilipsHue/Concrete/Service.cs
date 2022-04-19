using Dawn;

namespace Helpers.PhilipsHue.Concrete;

public class Service : IService
{
	private readonly IClient _client;

	public Service(IClient philipsHueClient)
	{
		_client = Guard.Argument(philipsHueClient).NotNull().Value;
	}

	public async Task SetLightStateAsync(string groupName, string lightName, Models.LightObject.StateObject state)
	{
		await foreach (var (_, group) in from kvp in _client.GetGroupsAsync()
										 let @group = kvp.Value
										 where @group.name.Equals(groupName, StringComparison.OrdinalIgnoreCase)
										 select kvp)
		{
			foreach (var lightId in group.lights)
			{
				var light = await _client.GetLightAsync(lightId);

				if (light.name.Equals(lightName, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				await _client.SetLightStateAsync(lightId, state);
			}
		}
	}

	public async Task ToggleLightsAsync(params string[] names)
	{
		await foreach (var (id, light) in GetLightsByNamesAsync(names))
		{
			var state = light.state with { on = !light.state.on, };
			await _client.SetLightStateAsync(id, state);
		}
	}

	public async Task SetLightsStateAsync(Models.LightObject.StateObject state, params string[] names)
	{
		await foreach (var (id, _) in GetLightsByNamesAsync(names))
		{
			await _client.SetLightStateAsync(id, state);
		}
	}

	public async Task ToggleLightsInGroupsAsync(params string[] groupNames)
	{
		await foreach (var (_, group) in GetGroupsByNamesAsync(groupNames))
		{
			foreach (var id in group.lights)
			{
				var light = await _client.GetLightAsync(id);
				var state = light.state with { on = !light.state.on, };
				await _client.SetLightStateAsync(id, state);
			}
		}
	}

	public async Task SetGroupsStateAsync(Models.LightObject.StateObject state, params string[] names)
	{
		await foreach (var (_, group) in GetGroupsByNamesAsync(names))
		{
			foreach (var id in group.lights)
			{
				await _client.SetLightStateAsync(id, state);
			}
		}
	}

	public IAsyncEnumerable<KeyValuePair<string, Models.LightObject>> GetLightsByNamesAsync(params string[] names)
	{
		return from kvp in _client.GetLightsAsync()
			   let light = kvp.Value
			   where names.Contains(light.name, StringComparer.OrdinalIgnoreCase)
			   select kvp;
	}

	public IAsyncEnumerable<KeyValuePair<string, Models.GroupObject>> GetGroupsByNamesAsync(params string[] names)
	{
		return from kvp in _client.GetGroupsAsync()
			   let light = kvp.Value
			   where names.Contains(light.name, StringComparer.OrdinalIgnoreCase)
			   select kvp;
	}
}
