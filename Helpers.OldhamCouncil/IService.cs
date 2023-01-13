namespace Helpers.OldhamCouncil;

public interface IService
{
	IAsyncEnumerable<KeyValuePair<string, string>> GetAddressesAsync(string postcode, CancellationToken cancellationToken = default);
	IAsyncEnumerable<KeyValuePair<DateTime, Models.BinTypes>> GetBinCollectionsAsync(string id, CancellationToken cancellationToken = default);
	IAsyncEnumerable<KeyValuePair<DateTime, Models.BinTypes>> GetBinCollectionsAsync(string postcode, string? houseNumber, CancellationToken cancellationToken = default);
}
