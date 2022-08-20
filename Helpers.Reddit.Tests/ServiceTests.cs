namespace Helpers.Reddit.Tests;

public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _sut;

	public ServiceTests(Fixtures.ServiceFixture serviceFixture)
	{
		_sut = serviceFixture.Service;
	}

	[Theory]
	[InlineData(10)]
	public async Task GetRandomSubredditTests(int count)
	{
		while (count-- > 0)
		{
			var actual = await _sut.GetRandomSubredditNameAsync();
			Assert.Matches("^[0-9A-Za-z]{2,}$", actual);
		}
	}

	[Theory]
	[InlineData("90dayfianceuncensored")]
	[InlineData("worldnews")]
	public async Task GetThreads(string subredditName)
	{
		// Act
		var threadIds = await _sut.GetThreadIdsForSubredditAsync(subredditName).ToListAsync();

		// Assert
		Assert.NotEmpty(threadIds);
		Assert.DoesNotContain(default, threadIds);
	}

	[Theory]
	[InlineData("euphoria", "cm3ryv")]
	public async Task GetComments(string subreddit, string threadId)
	{
		// Act
		var comments = await _sut.GetCommentsForThreadIdAsync(subreddit, threadId).ToListAsync();

		// Assert
		Assert.NotEmpty(comments);
		Assert.DoesNotContain(default, comments);
	}

	[Fact]
	public async Task EndToEndTests()
	{
		var comments = new List<string>();
		var subredditName = await _sut.GetRandomSubredditNameAsync();

		Assert.NotNull(subredditName);

		await foreach (var threadId in _sut.GetThreadIdsForSubredditAsync(subredditName))
		{
			await foreach (var comment in _sut.GetCommentsForThreadIdAsync(subredditName, threadId))
			{
				comments.Add(comment);
			}
		}

		Assert.NotEmpty(comments);
	}
}
