using System;
using System.Threading.Tasks;

namespace Helpers.SSH.Clients
{
	public interface ISSHClient : IDisposable
	{
		Task<string> RunCommandAsync(string commandText, int millisecondsTimeout = 5_000);
	}
}
