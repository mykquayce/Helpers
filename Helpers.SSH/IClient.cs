namespace Helpers.SSH;

public interface IClient
{
	Task<string> RunCommandAsync(string commandText, CancellationToken cancellationToken = default);
}
