using Helpers.DockerHub.Models;

namespace Helpers.DockerHub;

public interface IRegistryClient
{
	Task<ManifestsResponseObject> GetManifestsAsync(string tag, CancellationToken? cancellationToken = default);
	IAsyncEnumerable<string> GetTagsAsync(CancellationToken? cancellationToken = default);
}
