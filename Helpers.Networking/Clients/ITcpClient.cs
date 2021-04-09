using System.Collections.Generic;

namespace Helpers.Networking.Clients
{
	public interface ITcpClient
	{
		IAsyncEnumerable<string> SendAndReceiveAsync(string message);
	}
}
