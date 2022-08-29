using Dawn;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Helpers.Reddit.Concrete;

public partial class Client : IClient
{
	private readonly HttpClient _httpClient;
	private readonly XmlSerializerFactory _xmlSerializerFactory;

	public Client(HttpClient httpClient, XmlSerializerFactory xmlSerializerFactory)
	{
		_httpClient = Guard.Argument(httpClient).NotNull().Value;
		_xmlSerializerFactory = Guard.Argument(xmlSerializerFactory).NotNull().Value;
	}

	public async Task<string> GetRandomSubredditAsync(CancellationToken? cancellationToken = default)
	{
		Uri redirect;
		{
			using var request = new HttpRequestMessage(HttpMethod.Head, "r/random");
			using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken ?? CancellationToken.None);
			redirect = response.Headers.Location!;
		}
		var match = SubredditRegex().Match(redirect.OriginalString);
		return match.Groups[1].Value;
	}

	public async IAsyncEnumerable<Models.Generated.entryType> GetThreadsAsync(string subredditName, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(subredditName).IsSubredditName();

		Models.Generated.feedType subreddit;
		{
			await using var stream = await _httpClient.GetStreamAsync($"r/{subredditName}/.rss?limit=100", cancellationToken ?? CancellationToken.None);
			subreddit = Deserialize<Models.Generated.feedType>(stream);
		}

		var enumerator = subreddit.entry.GetEnumerator();

		while (cancellationToken?.IsCancellationRequested != true
			&& enumerator.MoveNext())
		{
			if (enumerator.Current is Models.Generated.entryType entry
				&& entry.Type == Models.EntryType.Link)
			{
				yield return entry;
			}
		}
	}

	public async IAsyncEnumerable<string> GetCommentsAsync(string subredditName, long threadId, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(subredditName).IsSubredditName();
		Models.Generated.feedType threadFeed;
		{
			var id = Helpers.Reddit.Models.Converters.Base36Converter.ToString(threadId);
			await using var stream = await _httpClient.GetStreamAsync($"r/{subredditName}/comments/{id}/.rss?limit=500", cancellationToken ?? CancellationToken.None);
			threadFeed = Deserialize<Models.Generated.feedType>(stream);
		}

		var enumerator = threadFeed.entry.GetEnumerator();

		while (cancellationToken?.IsCancellationRequested != true
			&& enumerator.MoveNext())
		{
			if (enumerator.Current is Models.Generated.entryType entry
				&& entry.Type == Models.EntryType.Comment)
			{
				var comment = entry.content.Value;
				if (string.IsNullOrWhiteSpace(comment)) continue;
				yield return System.Web.HttpUtility.HtmlDecode(comment);
			}
		}
	}

	private T Deserialize<T>(Stream stream)
	{
		Guard.Argument(stream).NotNull().Require(s => s.CanRead);
		var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
		return (T)serializer.Deserialize(stream)!;
	}

	[RegexGenerator("reddit\\.com\\/r\\/(\\w+)\\/")]
	private static partial Regex SubredditRegex();
}
