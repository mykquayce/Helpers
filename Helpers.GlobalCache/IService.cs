using System.Net;

namespace Helpers.GlobalCache;

public interface IService : IDisposable
{
	Task ConnectAsync(string uuidOrHostName);
	Task ConnectAsync(IPAddress ipAddress);
	Task ConnectWithUuidAsync(string uuid);
	Task ConnectWithHostNameAsync(string hostName);
	Task<string> SendAndReceiveAsync(string message, CancellationToken? cancellationToken = default);
}
