using System.Runtime.CompilerServices;

namespace Helpers.Reddit.Concrete;

public partial class Service(IClient client) : IService
{
	public Task<string> GetRandomSubredditNameAsync(CancellationToken cancellationToken = default)
		=> client.GetRandomSubredditAsync(cancellationToken);

	public IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken cancellationToken = default)
		=> client.GetThreadsAsync(subredditName, cancellationToken).Select(t => t.id);

	public IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken cancellationToken = default)
		=> client.GetCommentsAsync(subredditName, threadId, cancellationToken);
}
