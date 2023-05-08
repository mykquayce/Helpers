namespace Helpers.PhilipsHue;

public partial interface IClient
{
	IAsyncEnumerable<KeyValuePair<string, int>> GetGroupsAsync(CancellationToken cancellationToken = default);
	Task<bool> GetGroupPowerAsync(int group, CancellationToken cancellationToken = default);

	Task SetGroupPowerAsync(int group, bool on, CancellationToken cancellationToken = default)
		=> SetGroupPowerAsync(group, on, TimeSpan.Zero, cancellationToken);
	Task SetGroupPowerAsync(int group, bool on, TimeSpan transition, CancellationToken cancellationToken = default);

	Task ToggleGroupPowerAsync(int group, CancellationToken cancellationToken = default)
		=> ToggleGroupPowerAsync(group, TimeSpan.Zero, cancellationToken);
	async Task ToggleGroupPowerAsync(int group, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var on = await GetGroupPowerAsync(group, cancellationToken);
		await SetGroupPowerAsync(group, !on, transition, cancellationToken);
	}
}
