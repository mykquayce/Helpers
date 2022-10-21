namespace Helpers.Reddit;

public interface IService
{
	IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken cancellationToken = default)
		=> GetCommentsForThreadIdAsync(subredditName, Helpers.Reddit.Models.Converters.Base36Converter.FromString<long>(threadId), cancellationToken);
	IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, long threadId, CancellationToken cancellationToken = default);
	Task<string> GetRandomSubredditNameAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<long> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken cancellationToken = default);
}
