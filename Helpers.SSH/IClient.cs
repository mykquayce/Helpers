namespace Helpers.SSH;

public interface IClient : IDisposable
{
	Task<string> RunCommandAsync(string commandText, int millisecondsTimeout = 5_000);
	IAsyncEnumerable<string> RunCommandAsShellAsync(string commandText, CancellationToken cancellationToken = default);
}
