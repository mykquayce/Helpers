namespace Helpers.Reddit;

public interface IService
{
	IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken cancellationToken = default);
	IEnumerable<Uri> GetLinksFromComment(string comment);
	Task<string> GetRandomSubredditNameAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken cancellationToken = default);
}
