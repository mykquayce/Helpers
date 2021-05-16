using System.Collections.Generic;
using System.Net;

namespace Helpers.Networking.Clients
{
	public interface IWhoIsClient
	{
		IAsyncEnumerable<Models.AddressPrefix> GetIpsAsync(int asn);
		IAsyncEnumerable<Models.WhoIsResponse> GetWhoIsDetailsAsync(IPAddress ipAddress);
	}
}
