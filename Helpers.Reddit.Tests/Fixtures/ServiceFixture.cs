using Microsoft.Extensions.Options;

namespace Helpers.Reddit.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private static readonly List<string> _denylist = new()
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
		Service = new Concrete.Service(client, Options.Create(_denylist));
	}

	public IService Service { get; }

	public void Dispose() => _clientFixture.Dispose();
}
