using Dawn;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Helpers.Reddit.Concrete;

public class Service : IService
{
	private static readonly Regex _threadRegex = new(@"^https:\/\/old\.reddit\.com\/r\/(\w+)\/comments\/(\w+)\/(\w+)\/$");
	private static readonly Regex _linkRegex = new(@"\""(https?:\/\/.+?)\""");
	private readonly IClient _client;
	private readonly IReadOnlyCollection<string> _denylist;

	public Service(IClient client, IOptions<List<string>> denylistOptions)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_denylist = Guard.Argument(denylistOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().DoesNotContainNull().DoesNotContainDuplicate()
			.Value;
	}

	public Task<string> GetRandomSubredditNameAsync(CancellationToken? cancellationToken = default)
		=> _client.GetRandomSubredditNameAsync(cancellationToken);

	public async IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken? cancellationToken = default)
	{
		var threads = _client.GetThreadsFromSubredditAsync(subredditName, cancellationToken);

		await using var enumerator = threads.GetAsyncEnumerator(cancellationToken ?? CancellationToken.None);

		while (cancellationToken?.IsCancellationRequested != true
			&& await enumerator.MoveNextAsync())
		{
			var thread = enumerator.Current;
			var match = _threadRegex.Match(thread.link.href);
			var (_, _, threadId, _) = match;
			yield return threadId!;
		}
	}

	public IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken? cancellationToken = default)
		=> _client.GetCommentsFromThreadAsync(subredditName, threadId, cancellationToken);

	public IEnumerable<Uri> GetLinksFromComment(string comment, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(comment).NotNull().NotEmpty().NotWhiteSpace();

		var matches = _linkRegex.Matches(comment);
		var enumerator = matches.GetEnumerator();

		while (cancellationToken?.IsCancellationRequested != true
			&& enumerator.MoveNext())
		{
			var uriString = ((Match)enumerator.Current).Groups[1].Value;
			if (!Uri.TryCreate(uriString, UriKind.Absolute, out var uri)) continue;
			if (_denylist.Any(s => uri.Host.EndsWith(s, StringComparison.OrdinalIgnoreCase))) continue;
			yield return uri;
		}
	}
}
