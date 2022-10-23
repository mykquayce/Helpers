using System.Net;

namespace Helpers.GlobalCache;

public interface IService : IDisposable
{
	ValueTask ConnectAsync(string uuidOrHostName, CancellationToken cancellationToken = default);
	ValueTask ConnectAsync(IPAddress ipAddress, CancellationToken cancellationToken = default);
	ValueTask ConnectWithUuidAsync(string uuid, CancellationToken cancellationToken = default);
	ValueTask ConnectWithHostNameAsync(string hostName, CancellationToken cancellationToken = default);
	Task<string> SendAndReceiveAsync(string message, CancellationToken cancellationToken = default);
}
