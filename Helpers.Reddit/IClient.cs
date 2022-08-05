namespace Helpers.Reddit;

public interface IClient
{
	IAsyncEnumerable<string> GetCommentsAsync(string subreddit, string threadId, CancellationToken? cancellationToken = default);
	Task<string> GetRandomSubredditAsync(CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.Generated.entryType> GetThreadsAsync(string subredditName, CancellationToken? cancellationToken = default);
}
