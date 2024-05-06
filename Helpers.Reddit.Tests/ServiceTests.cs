namespace Helpers.Reddit.Tests;

public class ServiceTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IService _sut = fixture.Service;

	[Theory]
	[InlineData(10)]
	public async Task GetRandomSubredditTests(int count)
	{
		while (count-- > 0)
		{
			var actual = await _sut.GetRandomSubredditNameAsync();
			Assert.Matches("^[0-9A-Z_a-z]{2,}$", actual);
		}
	}

	[Theory]
	[InlineData("90dayfianceuncensored")]
	[InlineData("worldnews")]
	public async Task GetThreadIdsForSubredditTests(string subredditName)
	{
		// Act
		var threadIds = await _sut.GetThreadIdsForSubredditAsync(subredditName).Take(100).ToArrayAsync();

		// Assert
		Assert.NotEmpty(threadIds);
		Assert.DoesNotContain(default, threadIds);
	}

	[Theory]
	[InlineData("euphoria", "t3_cm3ryv", 100)]
	public async Task GetCommentsForThreadIdTests(string subreddit, string threadId, int count)
	{
		// Act
		var comments = await _sut.GetCommentsForThreadIdAsync(subreddit, threadId).Take(count).ToArrayAsync();

		// Assert
		Assert.NotEmpty(comments);
		Assert.Equal(count, comments.Length);
		Assert.DoesNotContain(default, comments);
	}
}
