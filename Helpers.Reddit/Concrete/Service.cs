using System.Text.RegularExpressions;

namespace Helpers.Reddit.Concrete;

public partial class Service(IClient client) : IService
{
	public Task<string> GetRandomSubredditNameAsync(CancellationToken cancellationToken = default)
		=> client.GetRandomSubredditAsync(cancellationToken);

	public IAsyncEnumerable<string> GetThreadIdsForSubredditAsync(string subredditName, CancellationToken cancellationToken = default)
		=> client.GetThreadsAsync(subredditName, cancellationToken).Select(t => t.id);

	public IAsyncEnumerable<string> GetCommentsForThreadIdAsync(string subredditName, string threadId, CancellationToken cancellationToken = default)
		=> client.GetCommentsAsync(subredditName, threadId, cancellationToken);

	public IEnumerable<Uri> GetLinksFromComment(string comment)
	{
		var matches = LinkRegex().Matches(comment);
		for (var a = 0; a < matches.Count; a++)
		{
			var uriString = matches[a].Groups[1].Value;
			if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
			{
				yield return uri!;
			}
		}
	}

	[GeneratedRegex(@"href=""(https:\/\/.+?)""", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline, matchTimeoutMilliseconds: 100)]
	private static partial Regex LinkRegex();
}
