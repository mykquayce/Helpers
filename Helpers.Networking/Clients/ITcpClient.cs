using System;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients
{
	public interface ITcpClient : IDisposable
	{
		Task<string> SendAndReceiveAsync(string message);
	}
}
