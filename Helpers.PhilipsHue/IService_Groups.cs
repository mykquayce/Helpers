namespace Helpers.PhilipsHue;

public partial interface IService
{
	Task<bool> GetGroupPowerAsync(string alias, CancellationToken cancellationToken = default);

	Task SetGroupPowerAsync(string alias, bool on, CancellationToken cancellationToken = default)
		=> SetGroupPowerAsync(alias, on, TimeSpan.Zero, cancellationToken);
	Task SetGroupPowerAsync(string alias, bool on, TimeSpan transition, CancellationToken cancellationToken = default);

	Task ToggleGroupPowerAsync(string alias, CancellationToken cancellationToken = default)
		=> ToggleGroupPowerAsync(alias, TimeSpan.Zero, cancellationToken);
	Task ToggleGroupPowerAsync(string alias, TimeSpan transition, CancellationToken cancellationToken = default);
}
