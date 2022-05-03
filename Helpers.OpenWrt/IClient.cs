namespace Helpers.OpenWrt;

public interface IClient
{
	Task<string> ExecuteCommandAsync(string command, CancellationToken? cancellationToken = default);
	Task<string> LoginAsync(CancellationToken? cancellationToken = default);
}
