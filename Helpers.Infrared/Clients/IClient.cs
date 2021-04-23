using System.Threading.Tasks;

namespace Helpers.Infrared.Clients
{
	public interface IClient
	{
		Task SendAsync(string host, string message);
	}
}
