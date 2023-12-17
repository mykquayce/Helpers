namespace Helpers.Reddit;

public interface IService
{
	IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken cancellationToken = default);
	Task<string> GetRandomSubredditNameAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken cancellationToken = default);
}
