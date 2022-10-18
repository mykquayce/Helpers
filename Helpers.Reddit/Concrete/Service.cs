using Dawn;

namespace Helpers.Reddit.Concrete;

public partial class Service : IService
{
	private readonly IClient _client;

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	public Task<string> GetRandomSubredditNameAsync(CancellationToken? cancellationToken = default)
		=> _client.GetRandomSubredditAsync(cancellationToken);

	public async IAsyncEnumerable<long> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken? cancellationToken = default)
	{
		var threads = _client.GetThreadsAsync(subredditName, cancellationToken);

		await using var enumerator = threads.GetAsyncEnumerator(cancellationToken ?? CancellationToken.None);

		while (cancellationToken?.IsCancellationRequested != true
			&& await enumerator.MoveNextAsync())
		{
			var thread = enumerator.Current;
			yield return thread.Id;
		}
	}

	public IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, long threadId, CancellationToken? cancellationToken = default)
		=> _client.GetCommentsAsync(subredditName, threadId, cancellationToken);
}
