namespace Helpers.Reddit;

public interface IClient
{
	IAsyncEnumerable<string> GetCommentsFromThreadAsync(string subreddit, string threadId, CancellationToken? cancellationToken = default);
	Task<string> GetRandomSubredditNameAsync(CancellationToken? cancellationToken = default);
	IAsyncEnumerable<Models.Generated.entryType> GetThreadsFromSubredditAsync(string subredditName, CancellationToken? cancellationToken = default);
}
