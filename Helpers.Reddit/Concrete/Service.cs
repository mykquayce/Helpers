using Dawn;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Text.RegularExpressions;

namespace Helpers.Reddit.Concrete;

public class Service : IService
{
	#region config
	public record Config(IReadOnlyCollection<string> Blacklist) : IOptions<Config>, IReadOnlyCollection<string>
	{
		#region IOptions implementation
		public Config Value => this;
		#endregion IOptions implementation

		#region IReadOnlyCollection implementation
		public int Count => Blacklist.Count;
		public IEnumerator<string> GetEnumerator() => Blacklist.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion IReadOnlyCollection implementation
	}
	#endregion config

	private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
	private readonly static Regex _linkRegex = new("(?:href|src)=\"(.+?)\"", _regexOptions);
	private readonly IClient _client;
	private readonly IEnumerable<string> _blacklist;

	public Service(IClient client, IOptions<Config> blacklistOptions)
	{
		_client = Guard.Argument(client).NotNull().Value;
		_blacklist = Guard.Argument(blacklistOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().DoesNotContainNull().Value;
	}

	public Task<string> GetRandomSubredditAsync() => _client.GetRandomSubredditAsync();

	public async IAsyncEnumerable<Models.IThread> GetThreadsAsync(string subreddit)
	{
		Guard.Argument(subreddit).IsSubredditName();

		await foreach (var thread in _client.GetThreadsAsync(subreddit))
		{
			yield return thread;
		}
	}

	public async IAsyncEnumerable<Models.IComment> GetCommentsAsync(string subreddit)
	{
		await foreach (var thread in GetThreadsAsync(subreddit))
		{
			await foreach (var comment in GetCommentsAsync(thread))
			{
				yield return comment;
			}
		}
	}

	public async IAsyncEnumerable<Models.IComment> GetCommentsAsync(Models.IThread thread)
	{
		Guard.Argument(thread).NotNull();
		Guard.Argument(thread.Subreddit).IsSubredditName();
		Guard.Argument(thread.Id).IsThreadId();

		await foreach (var entry in _client.GetCommentsAsync(thread.Subreddit, thread.Id))
		{
			yield return entry;
		}
	}

	public async IAsyncEnumerable<Uri> GetUrisAsync(string subreddit)
	{
		await foreach (var thread in GetThreadsAsync(subreddit))
		{
			await foreach (var uri in GetUrisAsync(thread))
			{
				yield return uri;
			}
		}
	}

	public async IAsyncEnumerable<Uri> GetUrisAsync(Models.IThread thread)
	{
		await foreach (var comment in GetCommentsAsync(thread))
		{
			foreach (var uri in GetUris(comment))
			{
				yield return uri;
			}
		}
	}

	public IEnumerable<Uri> GetUris(Models.IComment comment)
	{
		var matches = _linkRegex.Matches(comment.Content);

		foreach (Match match in matches)
		{
			var uriString = match.Groups[1].Value;

			var ok = Uri.TryCreate(uriString, UriKind.Absolute, out var result);

			if (ok)
			{
				if (_blacklist.Any(s => result!.Host.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}

				yield return result!;
			}
		}
	}
}
