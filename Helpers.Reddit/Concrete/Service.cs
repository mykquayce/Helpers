using Dawn;
using System.Text.RegularExpressions;

namespace Helpers.Reddit.Concrete;

public class Service : IService
{
	private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

	private readonly static IEnumerable<string> _blacklist = new[]
	{
			"redd.it",
			"reddit.com",
			"redditmedia.com",
			"redditsave.com",

			"amazon.com",
			"apple.com",
			"discord.gg",
			"discordapp.com",
			"discordapp.net",
			"giphy.com",
			"google.com",
			"gfycat.com",
			"imgflip.com",
			"imgur.com",
			"spotify.com",
			"twimg.com",
			"twitch.tv",
			"twitter.com",
			"youtube.com",
			"youtu.be",
		};

	private readonly static IEnumerable<Regex> _regices = new Regex[1]
	{
			new("(?:href|src)=\"(.+?)\"", _regexOptions),
	};

	public Task<string> GetRandomSubredditAsync() => _client.GetRandomSubredditAsync();

	private readonly IClient _client;

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	public async IAsyncEnumerable<Models.IThread> GetThreadsAsync(string subreddit)
	{
		Guard.Argument(subreddit).IsSubredditName();

		await foreach (var thread in _client.GetThreadsAsync(subreddit))
		{
			yield return (Models.Concrete.Entry)thread;
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
			yield return (Models.Concrete.Entry)entry;
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
		foreach (var regex in _regices)
		{
			var matches = regex.Matches(comment.Content);

			foreach (Match match in matches)
			{
				var uriString = match.Groups[1].Value;

				var ok = Uri.TryCreate(uriString, UriKind.Absolute, out var result);

				if (ok)
				{
					if (_blacklist.Any(s => result!.Host.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)))
					{
						continue;
					}

					yield return result!;
				}
			}
		}
	}

	#region dispose
	private bool _disposed;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_client.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion dispose
}
