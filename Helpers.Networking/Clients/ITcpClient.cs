using System.Threading.Tasks;

namespace Helpers.Networking.Clients
{
	public interface ITcpClient
	{
		Task<string> SendAndReceiveAsync(string message);
	}
}
