namespace Helpers.Cineworld;

public interface IClient
{
	Task<Models.Generated.AllPerformances.cinemas> GetAllPerformancesAsync(CancellationToken? cancellationToken = default);
	Task<Models.Generated.Listings.cinemas> GetListingsAsync(CancellationToken? cancellationToken = default);
}
