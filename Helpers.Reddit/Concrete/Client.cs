using Dawn;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Helpers.Reddit.Concrete;

public class Client : IClient
{
	private static readonly Regex _subredditRegex = new(@"reddit\.com\/r\/(\w+)\/");
	private readonly HttpClient _httpClient;
	private readonly XmlSerializerFactory _xmlSerializerFactory;

	public Client(HttpClient httpClient, XmlSerializerFactory xmlSerializerFactory)
	{
		_httpClient = Guard.Argument(httpClient).NotNull().Value;
		_xmlSerializerFactory = Guard.Argument(xmlSerializerFactory).NotNull().Value;
	}

	public async Task<string> GetRandomSubredditNameAsync(CancellationToken? cancellationToken = default)
	{
		Uri redirect;
		{
			using var response = await _httpClient.GetAsync("r/random", HttpCompletionOption.ResponseHeadersRead, cancellationToken ?? CancellationToken.None);
			redirect = response.Headers.Location!;
		}
		var match = _subredditRegex.Match(redirect.OriginalString);
		return match.Groups[1].Value;
	}

	public async IAsyncEnumerable<Models.Generated.entryType> GetThreadsFromSubredditAsync(string subredditName, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(subredditName).IsSubredditName();

		Models.Generated.feedType subreddit;
		{
			await using var stream = await _httpClient.GetStreamAsync($"r/{subredditName}/.rss", cancellationToken ?? CancellationToken.None);
			subreddit = Deserialize<Models.Generated.feedType>(stream);
		}

		var enumerator = subreddit.entry.GetEnumerator();

		while (cancellationToken?.IsCancellationRequested != true
			&& enumerator.MoveNext())
		{
			yield return (Models.Generated.entryType)enumerator.Current;
		}
	}

	public async IAsyncEnumerable<string> GetCommentsFromThreadAsync(string subredditName, string threadId, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(subredditName).IsSubredditName();
		Guard.Argument(threadId).IsThreadId();
		Models.Generated.feedType threadFeed;
		{
			await using var stream = await _httpClient.GetStreamAsync($"r/{subredditName}/comments/{threadId}/.rss", cancellationToken ?? CancellationToken.None);
			threadFeed = Deserialize<Models.Generated.feedType>(stream);
		}

		var enumerator = threadFeed.entry.GetEnumerator();

		while (cancellationToken?.IsCancellationRequested != true
			&& enumerator.MoveNext())
		{
			var entry = (Models.Generated.entryType)enumerator.Current;
			var comment = entry.content.Value;
			if (string.IsNullOrWhiteSpace(comment)) continue;
			yield return System.Web.HttpUtility.HtmlDecode(comment);
		}
	}

	private T Deserialize<T>(Stream stream)
	{
		Guard.Argument(stream).NotNull().Require(s => s.CanRead);
		var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
		return (T)serializer.Deserialize(stream)!;
	}
}
