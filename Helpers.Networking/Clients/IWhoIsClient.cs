using System.Collections.Generic;
using System.Net;

namespace Helpers.Networking.Clients
{
	public interface IWhoIsClient
	{
		IAsyncEnumerable<Models.SubnetAddress> GetIpsAsync(int asn);
	}
}
