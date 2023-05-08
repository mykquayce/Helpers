namespace Helpers.PhilipsHue;

public partial interface IService
{
	Task<bool> GetGroupPowerAsync(string alias, CancellationToken cancellationToken = default);
	Task SetGroupPowerAsync(string alias, bool on, CancellationToken cancellationToken = default);
	Task ToggleGroupPowerAsync(string alias, CancellationToken cancellationToken = default);
}
