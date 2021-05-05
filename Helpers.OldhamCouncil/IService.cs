using System;
using System.Collections.Generic;
using System.Threading;

namespace Helpers.OldhamCouncil
{
	public interface IService
	{
		IAsyncEnumerable<KeyValuePair<string, long>> GetAddressesAsync(string postcode, CancellationToken? cancellationToken = default);
		IAsyncEnumerable<KeyValuePair<DateTime, Models.BinTypes>> GetBinCollectionsAsync(long id, CancellationToken? cancellationToken = default);
		IAsyncEnumerable<KeyValuePair<DateTime, Models.BinTypes>> GetBinCollectionsAsync(string postcode, string? houseNumber, CancellationToken? cancellationToken = default);
	}
}
