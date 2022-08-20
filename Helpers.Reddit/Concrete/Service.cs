using Dawn;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

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

	public async IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken? cancellationToken = default)
	{
		var threads = _client.GetThreadsAsync(subredditName, cancellationToken);

		await using var enumerator = threads.GetAsyncEnumerator(cancellationToken ?? CancellationToken.None);

		while (cancellationToken?.IsCancellationRequested != true
			&& await enumerator.MoveNextAsync())
		{
			var thread = enumerator.Current;
			var match = ThreadRegex().Match(thread.link.href);

			if (match.Success)
			{
				var (_, _, threadId, _) = match;
				yield return threadId!;
			}
		}
	}

	public IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken? cancellationToken = default)
		=> _client.GetCommentsAsync(subredditName, threadId, cancellationToken);

	[RegexGenerator("^https:\\/\\/old\\.reddit\\.com\\/r\\/(\\w+)\\/comments\\/(\\w+)\\/(\\w+)\\/$")]
	private static partial Regex ThreadRegex();

	[RegexGenerator("\\\"(https?:\\/\\/.+?)\\\"")]
	private static partial Regex LinkRegex();
}
