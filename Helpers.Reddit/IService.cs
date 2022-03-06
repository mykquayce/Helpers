namespace Helpers.Reddit;

public interface IService
{
	IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken? cancellationToken = default);
	IEnumerable<Uri> GetLinksFromComment(string comment, CancellationToken? cancellationToken = default);
	Task<string> GetRandomSubredditNameAsync(CancellationToken? cancellationToken = default);
	IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken? cancellationToken = default);

	async IAsyncEnumerable<Uri> GetLinksForThreadIdAsync(string subredditName, string threadId, CancellationToken? cancellationToken = default)
	{
		var comments = GetCommentsForThreadIdAsync(subredditName, threadId, cancellationToken);

		await foreach (var comment in comments)
		{
			var links = GetLinksFromComment(comment, cancellationToken);

			foreach (var link in links)
			{
				yield return link;
			}
		}
	}
}
