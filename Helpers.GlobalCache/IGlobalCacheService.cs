using System;
using System.Threading.Tasks;

namespace Helpers.GlobalCache
{
	public interface IService : IDisposable
	{
		Task<string> ConnectSendReceiveAsync(string hostName, string message);
	}
}
