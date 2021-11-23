namespace Helpers.DockerHub;

public interface IAuthorizationClient
{
	Task<string> GetTokenAsync(string organization, string repository, CancellationToken? cancellationToken = default);
	Task<(string token, DateTime expires)> GetTokenFromRemoteAsync(string organization, string repository, CancellationToken? cancellationToken = default);
}
