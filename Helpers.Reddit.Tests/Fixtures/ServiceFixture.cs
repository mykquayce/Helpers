namespace Helpers.Reddit.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private static readonly IReadOnlyCollection<string> _blacklist = new[]
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

	private readonly ClientFixture _clientFixture = new();

	public ServiceFixture()
	{
		var client = _clientFixture.Client;
		Service = new Concrete.Service(client, _blacklist);
	}

	public IService Service { get; }

	public void Dispose() => _clientFixture.Dispose();
}
