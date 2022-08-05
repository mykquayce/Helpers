using Dawn;
using System.Diagnostics;

namespace Helpers.Reddit.Tests;

public class ClientTests : IClassFixture<Fixtures.ClientFixture>
{
	public readonly IClient _sut;

	public ClientTests(Fixtures.ClientFixture clientFixture)
	{
		_sut = clientFixture.Client;
	}

	[Theory]
	[InlineData("worldnews")]
	public async Task GetThreads(string subredditName)
	{
		var threads = await _sut.GetThreadsAsync(subredditName).ToListAsync();

		Assert.NotEmpty(threads);
		Assert.DoesNotContain(default, threads);
	}

	[Theory]
	[InlineData("euphoria", "cm3ryv")]
	public async Task GetComments(string subredditName, string threadId)
	{
		var comments = await _sut.GetCommentsAsync(subredditName, threadId).ToListAsync();

		Assert.NotEmpty(comments);
		Assert.DoesNotContain(default, comments);
	}

	[Theory]
	[InlineData(10)]
	public async Task TestSubredditNames(int count)
	{
		while (count-- > 0)
		{
			var subreddit = await _sut.GetRandomSubredditAsync();

			try
			{
				Guard.Argument(subreddit).IsSubredditName();
			}
			catch (Exception ex)
			{
				Assert.True(false, ex.Message);
			}
		}
	}

	[Theory]
	[InlineData("/r/worldnews/.rss")]
	[InlineData("/r/worldnews/comments/mplu2s/.rss")]
	[InlineData("/r/worldnews/comments/mplu2s/guaobq4/.rss")]
	[InlineData("/r/worldnews/comments/mplu2s/scientists_4c_would_unleash_unimaginable_amounts/.rss")]
	[InlineData("/r/worldnews/comments/mplu2s/scientists_4c_would_unleash_unimaginable_amounts/guaobq4/.rss")]
	public async Task TimeoutTests(string uriString)
	{
		var uri = new Uri(uriString, UriKind.Relative);
		var baseAddress = new Uri("https://old.reddit.com", UriKind.Absolute);

		var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var client = new HttpClient(handler) { BaseAddress = baseAddress, };

		var stopwatch = Stopwatch.StartNew();
		var s = await client.GetStringAsync(uri);
		stopwatch.Stop();
		Console.WriteLine(stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerSecond);
	}

	[Theory]
	[InlineData("Superstonk", "ptvaka")]
	public async Task ThreadWithNullCommentTests(string subreddit, string threadId)
	{
		var comments = await _sut.GetCommentsAsync(subreddit, threadId)
			.ToListAsync();

		Assert.NotNull(comments);
		Assert.NotEmpty(comments);
		Assert.DoesNotContain(default, comments);
	}
}
