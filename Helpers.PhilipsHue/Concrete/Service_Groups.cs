namespace Helpers.PhilipsHue.Concrete;

public partial class Service
{
	public async Task<bool> GetGroupPowerAsync(string alias, CancellationToken cancellationToken = default)
	{
		var id = await ResolveGroupNameAsync(alias, cancellationToken);
		return await _client.GetGroupPowerAsync(id, cancellationToken);
	}

	public async Task SetGroupPowerAsync(string alias, bool on, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var id = await ResolveGroupNameAsync(alias, cancellationToken);
		await _client.SetGroupPowerAsync(id, on, transition, cancellationToken);
	}

	public async Task ToggleGroupPowerAsync(string alias, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var id = await ResolveGroupNameAsync(alias, cancellationToken);
		await _client.ToggleGroupPowerAsync(id, transition, cancellationToken);
	}
}
