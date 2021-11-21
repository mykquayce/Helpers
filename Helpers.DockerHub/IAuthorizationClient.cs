namespace Helpers.DockerHub;

public interface IAuthorizationClient
{
	string Organization { get; }
	string Repository { get; }
	Task<string> GetTokenAsync(CancellationToken? cancellationToken = default);
	Task<(string token, DateTime expires)> GetTokenFromRemoteAsync(CancellationToken? cancellationToken = default);
}
