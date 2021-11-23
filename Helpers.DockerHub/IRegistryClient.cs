using Helpers.DockerHub.Models;

namespace Helpers.DockerHub;

public interface IRegistryClient
{
	Task<ManifestsResponseObject> GetManifestsAsync(string organization, string repository, string tag, CancellationToken? cancellationToken = default);
	IAsyncEnumerable<string> GetTagsAsync(string organization, string repository, CancellationToken? cancellationToken = default);
}
