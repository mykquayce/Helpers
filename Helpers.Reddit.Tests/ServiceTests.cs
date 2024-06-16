using System.Text.RegularExpressions;

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
	[InlineData("90dayfianceuncensored", 120)]
	[InlineData("worldnews", 120)]
	public async Task GetThreadIdsForSubredditTests(string subredditName, int count)
	{
		// Act
		var threadIds = await _sut.GetThreadIdsForSubredditAsync(subredditName).Take(count).ToArrayAsync();

		// Assert
		Assert.NotEmpty(threadIds);
		Assert.DoesNotContain(default, threadIds);
		Assert.Equal(count, threadIds.Length);
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
		// at least 90% of comments are unique (some will just be [deleted])
		Assert.InRange(comments.Distinct().Count(), count * .9, count);
	}

	[Theory]
	[InlineData("""<a href="https://preview.redd.it/24vk5mf0mnzc1.jpeg?width=554&format=pjpg&auto=webp&s=e6ad3163769c8034cf656c3e4d6d2a4e0516ed1d">https://preview.redd.it/24vk5mf0mnzc1.jpeg?width=554&format=pjpg&auto=webp&s=e6ad3163769c8034cf656c3e4d6d2a4e0516ed1d</a>""")]
	[InlineData("""Glad that I was able to hurt your feelings!</p> <p><a href="https://giphy.com/gifs/Im6tajjZwrRN6QQx75">https://giphy.com/gifs/Im6tajjZwrRN6QQx75</a>""")]
	[InlineData("""<a href="https://preview.redd.it/n3bpc8zbmnzc1.jpeg?width=320&format=pjpg&auto=webp&s=2c3f6910ec90ffb74fa12017fe3864f3938f90ac">https://preview.redd.it/n3bpc8zbmnzc1.jpeg?width=320&format=pjpg&auto=webp&s=2c3f6910ec90ffb74fa12017fe3864f3938f90ac</a>""")]
	[InlineData("""<a href="https://preview.redd.it/ux7mcppzmnzc1.jpeg?width=720&format=pjpg&auto=webp&s=d008106f223b01517379b9e15f8a7b8a19e86cad">https://preview.redd.it/ux7mcppzmnzc1.jpeg?width=720&format=pjpg&auto=webp&s=d008106f223b01517379b9e15f8a7b8a19e86cad</a></p> <p>I don't know what to reply with, so here's a meme!""")]
	public void GetLinksFromCommentsTests(string comment)
	{
		var regex = new Regex(@"href=""(https:\/\/.+?)""");
		var link = regex.Match(comment).Groups[1].Value;
		Assert.NotEmpty(link);
		Assert.True(Uri.TryCreate(link, UriKind.Absolute, out _));
	}
}
