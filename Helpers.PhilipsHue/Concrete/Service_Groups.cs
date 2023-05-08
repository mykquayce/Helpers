namespace Helpers.PhilipsHue.Concrete;

public partial class Service
{
	public async Task<bool> GetGroupPowerAsync(string alias, CancellationToken cancellationToken = default)
	{
		var groups = _client.GetGroupsAsync(cancellationToken);
		await foreach (var (name, id) in groups)
		{
			if (string.Equals(alias, name, StringComparison.OrdinalIgnoreCase))
			{
				return await _client.GetGroupPowerAsync(id, cancellationToken);
			}
		}
		throw new ArgumentOutOfRangeException(nameof(alias), alias, $"group {alias} not found");
	}

	public async Task SetGroupPowerAsync(string alias, bool on, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var groups = _client.GetGroupsAsync(cancellationToken);
		await foreach (var (name, id) in groups)
		{
			if (string.Equals(alias, name, StringComparison.OrdinalIgnoreCase))
			{
				await _client.SetGroupPowerAsync(id, on, transition, cancellationToken);
				return;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(alias), alias, $"group {alias} not found");
	}

	public async Task ToggleGroupPowerAsync(string alias, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var groups = _client.GetGroupsAsync(cancellationToken);
		await foreach (var (name, id) in groups)
		{
			if (string.Equals(alias, name, StringComparison.OrdinalIgnoreCase))
			{
				await _client.ToggleGroupPowerAsync(id, transition, cancellationToken);
				return;
			}
		}
		throw new ArgumentOutOfRangeException(nameof(alias), alias, $"group {alias} not found");
	}
}
