using System.Collections.Generic;
using System.Threading;

namespace Helpers.OldhamCouncil
{
	public interface IClient
	{
		IAsyncEnumerable<Models.Address> GetAddressesAsync(string postcode, CancellationToken cancellationToken = default);
		IAsyncEnumerable<Models.Generated.tableType> GetBinCollectionsAsync(string uprn, CancellationToken cancellationToken = default);
	}
}
