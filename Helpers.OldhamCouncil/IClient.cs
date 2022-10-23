using System.Collections.Generic;
using System.Threading;

namespace Helpers.OldhamCouncil
{
	public interface IClient
	{
		IAsyncEnumerable<KeyValuePair<long, string>> GetAddressesAsync(string postcode, CancellationToken cancellationToken = default);
		IAsyncEnumerable<Models.Generated.tableType> GetBinCollectionsAsync(long id, CancellationToken cancellationToken = default);
	}
}
