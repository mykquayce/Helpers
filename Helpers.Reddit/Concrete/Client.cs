using Dawn;
using System.Runtime.CompilerServices;

namespace Helpers.Reddit.Concrete;

public class Client(HttpClient httpClient) : IClient
{
	public async Task<string> GetRandomSubredditAsync(CancellationToken cancellationToken = default)
	{
		var response = await httpClient.GetAsync("/r/random", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
		return response.Headers.Location!.Segments.Last().Trim('/');
	}

	public async IAsyncEnumerable<Models.Generated.entryType> GetThreadsAsync(string subredditName, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		Guard.Argument(subredditName).IsSubredditName();

		string? after = null;

		do
		{
			var requestUri = new Uri($"/r/{subredditName}/.xml?after={after}&limit=100", UriKind.Relative);
			var feed = await httpClient.GetFromXml<Models.Generated.feedType>(requestUri, cancellationToken);

			foreach (var entry in feed.entry)
			{
				if (entry.id[1] != '3') { continue; } // link
				yield return entry;
				after = entry.id;
			}
		}
		while (after != null);
	}

	public async IAsyncEnumerable<string> GetCommentsAsync(string subredditName, string threadId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		Guard.Argument(subredditName).IsSubredditName();
		Guard.Argument(threadId).IsId();

		var requestUri = new Uri($"r/{subredditName}/comments/{threadId[3..]}/.rss?&limit=500", UriKind.Relative);
		var feed = await httpClient.GetFromXml<Models.Generated.feedType>(requestUri, cancellationToken);

		foreach (var entry in feed.entry)
		{
			if (entry.id[1] != '1') { continue; } // comment
			var comment = System.Web.HttpUtility.HtmlDecode(entry.content.Value);
			yield return comment;
		}
	}
}
