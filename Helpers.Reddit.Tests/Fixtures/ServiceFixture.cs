namespace Helpers.Reddit.Tests.Fixtures;

public class ServiceFixture : ClientFixture
{
	private readonly static IReadOnlyCollection<string> _blacklist = new[]
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

	public ServiceFixture()
	{
		var config = new Concrete.Service.Config(_blacklist);
		Service = new Concrete.Service(base.Client, config);
	}

	public Helpers.Reddit.IService Service { get; }
}
