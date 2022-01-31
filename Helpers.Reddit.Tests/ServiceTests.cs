using Xunit;

namespace Helpers.Reddit.Tests;

public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _sut;

	public ServiceTests(Fixtures.ServiceFixture serviceFixture)
	{
		_sut = serviceFixture.Service;
	}

	[Theory]
	[InlineData(
		"<!-- SC_OFF --><div class=\"md\"><p>*** People are being approached with private messages by others posing as Exodus support and then falling prey to scams and losing all their crypto.</p> <p>Exodus support will <strong>NEVER</strong> ask for any personal information.<br/> Exodus support will <strong>NEVER</strong> ask for your password or secret 12-word phrase. <strong>NEVER</strong> give out this information to anyone.<br/> Exodus support will <strong>NEVER</strong> send you to a website to enter your 12-word phrase.</p> <p>These are all scams and your money will be gone with no way to recover it.<br/> *** KEEP ALL YOUR WALLET INFORMATION PRIVATE AND DO NOT SHARE WITH ANYONE!!<br/> *** DO NOT DOWNLOAD ANYTHING FROM A SITE EXCEPT FROM OUR OFFICIAL WEBSITE <a href=\"https://exodus.com\">https://exodus.com</a><br/> *** DO NOT ENTER YOUR 12-WORDS INTO ANY WEBSITE!</p> <p>If someone approaches you in a private message, please contact a moderator.</p> </div><!-- SC_ON --> &#32; submitted by &#32; <a href=\"https://old.reddit.com/user/D-Day_68\"> /u/D-Day_68 </a> <br/> <span><a href=\"https://old.reddit.com/r/ExodusWallet/comments/layv5v/warning_scams_in_exodus_reddit/\">[link]</a></span> &#32; <span><a href=\"https://old.reddit.com/r/ExodusWallet/comments/layv5v/warning_scams_in_exodus_reddit/\">[comments]</a></span>",
		"https://exodus.com"
		)]
	[InlineData(
		"<!-- SC_OFF --><div class=\"md\"><p>There are a couple that on Sideline Swap that are new in the box 2018 upper mid-range models that may fit you.</p> <p>CCM Tacks 9090 9.5EE for $250US (originally around $550): <a href=\"https://sidelineswap.com/gear/hockey/skates/player-skates/2916040-ccm-new-tacks-9090-ice-hockey-player-skates-senior-9-5-ee-wide-width-skate-sr\">https://sidelineswap.com/gear/hockey/skates/player-skates/2916040-ccm-new-tacks-9090-ice-hockey-player-skates-senior-9-5-ee-wide-width-skate-sr</a></p> <p>Bauer Supreme Ignite Pro 9.5EE for $280US (an upgraded SMU of the S29; so a $400+ skate when new): <a href=\"https://sidelineswap.com/gear/hockey/skates/player-skates/2274045-bauer-new-ignite-pro-extra-wide-width-size-9-5-hockey-skates\">https://sidelineswap.com/gear/hockey/skates/player-skates/2274045-bauer-new-ignite-pro-extra-wide-width-size-9-5-hockey-skates</a></p> <p>or Icewarehouse has the True TF7s for $310US that are a full one piece carbon Fiber boots and IMHO the best bang for the buck skate currently at retail. They have them in 8.5W and 9W; you could order both and send the ones that don&#39;t fit back: <a href=\"https://www.icewarehouse.com/True_TF7/descpage-TF7SK.html\">https://www.icewarehouse.com/True_TF7/descpage-TF7SK.html</a></p> <p>If you&#39;re in Canada, the Hockey shop has them for $319 CAD (usually $400, but has the features of a much more expensive skate): <a href=\"https://www.thehockeyshop.com/products/true-tf7-senior-hockey-skates\">https://www.thehockeyshop.com/products/true-tf7-senior-hockey-skates</a></p> <p>I don&#39;t know what your budget is, and maybe someone will have something that will fit you for less, but those the skates above are all solid mid-range skates for around $300 or less that should provide plenty of support and last a while.</p> </div><!-- SC_ON -->",
		"https://sidelineswap.com/gear/hockey/skates/player-skates/2916040-ccm-new-tacks-9090-ice-hockey-player-skates-senior-9-5-ee-wide-width-skate-sr",
		"https://sidelineswap.com/gear/hockey/skates/player-skates/2274045-bauer-new-ignite-pro-extra-wide-width-size-9-5-hockey-skates",
		"https://www.icewarehouse.com/True_TF7/descpage-TF7SK.html",
		"https://www.thehockeyshop.com/products/true-tf7-senior-hockey-skates"
		)]
	public void GetUris(string comment, params string[] expected)
	{
		// Act
		var uris = _sut.GetLinksFromComment(comment).ToList();

		// Assert
		Assert.NotNull(uris);
		Assert.DoesNotContain(default, uris);
		var actual = uris.Select(uri => uri.OriginalString).ToList();
		Assert.Equal(expected, actual);
	}

	[Theory]
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
		var uris = new List<Uri>();
		var subredditName = await _sut.GetRandomSubredditNameAsync();

		Assert.NotNull(subredditName);

		await foreach (var threadId in _sut.GetThreadIdsForSubredditAsync(subredditName))
		{
			await foreach (var comment in _sut.GetCommentsForThreadIdAsync(subredditName, threadId))
			{
				foreach (var uri in _sut.GetLinksFromComment(comment))
				{
					uris.Add(uri);
				}
			}
		}

		Assert.NotEmpty(uris);
	}
}
