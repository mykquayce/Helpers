namespace Helpers.Reddit;

public interface IService
{
	Task<string> GetRandomSubredditAsync();
	IAsyncEnumerable<Models.IThread> GetThreadsAsync(string subreddit);
	IAsyncEnumerable<Models.IComment> GetCommentsAsync(Models.IThread thread);
	IEnumerable<Uri> GetUris(Models.IComment comment);
}
