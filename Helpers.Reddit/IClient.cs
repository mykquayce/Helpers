namespace Helpers.Reddit;

public interface IClient
{
	IAsyncEnumerable<string> GetCommentsAsync(string subreddit, string threadId, CancellationToken cancellationToken = default)
		=> GetCommentsAsync(subreddit, Helpers.Reddit.Models.Converters.Base36Converter.FromString<long>(threadId), cancellationToken);
	IAsyncEnumerable<string> GetCommentsAsync(string subreddit, long threadId, CancellationToken cancellationToken = default);
	Task<string> GetRandomSubredditAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<Models.Generated.entryType> GetThreadsAsync(string subredditName, CancellationToken cancellationToken = default);
}
