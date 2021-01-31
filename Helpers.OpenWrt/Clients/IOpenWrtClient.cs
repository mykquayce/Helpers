using System;
using System.Threading.Tasks;

namespace Helpers.OpenWrt.Clients
{
	public interface IOpenWrtClient : IDisposable
	{
		Task<string> ExecuteCommandAsync(string command);
		Task<string> LoginAsync();
	}
}
