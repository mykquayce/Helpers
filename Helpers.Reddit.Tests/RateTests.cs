using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Reddit.Tests;

public class RateTests
{
	private static readonly IReadOnlyCollection<string> _hostDenyList =
	[
		"redd.it",
		"reddit.com",
		"redditmedia.com",
		"redditsave.com",
	];

	private static readonly Uri _baseAddress = new("https://old.reddit.com/r/", UriKind.Absolute);
	private const string _userAgent = "Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405";


	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "work in progress")]
	[Theory(Skip = "work in progress")]
	[InlineData("00:00:05", 1, 1)]
	public async Task Test1(string replenishmentPeriod, int tokenLimit, int tokensPerPeriod)
	{
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddRateLimitHandler(TimeSpan.Parse(replenishmentPeriod), tokenLimit, tokensPerPeriod)
			.AddUserAgentHandler(_userAgent)
			.AddReddit(baseAddress: _baseAddress)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<UserAgentHandler>()
				.AddHttpMessageHandler<RateLimitHandler>()
				.Services
			.BuildServiceProvider();

		ICollection<Uri> links = [];
		var sut = provider.GetRequiredService<IService>();

		// Act
		var subreddit = await sut.GetRandomSubredditNameAsync();
		await foreach (var thread in sut.GetThreadIdsForSubredditAsync(subreddit).Take(100))
		{
			await foreach (var comment in sut.GetCommentsForThreadIdAsync(subreddit, thread))
			{
				foreach (var link in sut.GetLinksFromComment(comment))
				{
					if (_hostDenyList.Any(s => link.Host.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
					{
						continue;
					}

					links.Add(link);
				}
			}
		}

		// Assert
	}
}
