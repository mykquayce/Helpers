namespace Helpers.Reddit;

public interface IClient : IDisposable
{
	IAsyncEnumerable<Models.Generated.entry> GetCommentsAsync(string subreddit, string threadId);
	Task<string> GetRandomSubredditAsync();
	IAsyncEnumerable<Models.Generated.entry> GetThreadsAsync(string subreddit);
}
