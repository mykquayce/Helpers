using System.Collections.Generic;

namespace Helpers.Networking.Clients
{
	public interface IWhoIsClient
	{
		IAsyncEnumerable<Models.SubnetAddress> GetIpsAsync(int asn);
	}
}
