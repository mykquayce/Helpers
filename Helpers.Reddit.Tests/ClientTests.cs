using Dawn;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Tests;

public class ClientTests : IClassFixture<Fixtures.ClientFixture>
{
	public readonly Helpers.Reddit.IClient _sut;

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
	public async Task GetComments(string thread, string id)
	{
		var comments = await _sut.GetCommentsAsync(thread, id).ToListAsync();

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
	[InlineData(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<entry xmlns=""http://www.w3.org/2005/Atom"">
	<author>
		<name>/u/dookiea</name>
		<uri>https://old.reddit.com/user/dookiea</uri>
	</author>
	<category term=""worldnews"" label=""r/worldnews""/>
	<content type=""html"">&amp;#32; submitted by &amp;#32; &lt;a href=&quot;https://old.reddit.com/user/dookiea&quot;&gt; /u/dookiea &lt;/a&gt; &lt;br/&gt; &lt;span&gt;&lt;a href=&quot;https://www.yahoo.com/news/citing-grave-threat-scientific-american-replacing-climate-change-with-climate-emergency-181629578.html?guccounter=1&amp;amp;guce_referrer=aHR0cHM6Ly9vbGQucmVkZGl0LmNvbS8_Y291bnQ9MjI1JmFmdGVyPXQzX21waHF0ZA&amp;amp;guce_referrer_sig=AQAAAFucvBEBUIE14YndFzSLbQvr0DYH86gtanl0abh_bDSfsFVfszcGr_AqjlS2MNGUwZo23D9G2yu9A8wGAA9QSd5rpqndGEaATfXJ6uJ2hJS-ZRNBfBSVz1joN7vbqojPpYolcG6j1esukQ4BOhFZncFuGa9E7KamGymelJntbXPV&quot;&gt;[link]&lt;/a&gt;&lt;/span&gt; &amp;#32; &lt;span&gt;&lt;a href=&quot;https://old.reddit.com/r/worldnews/comments/mprsni/citing_grave_threat_scientific_american_replaces/&quot;&gt;[comments]&lt;/a&gt;&lt;/span&gt;</content>
	<id>t3_mprsni</id>
	<link href=""https://old.reddit.com/r/worldnews/comments/mprsni/citing_grave_threat_scientific_american_replaces/"" />
	<updated>2021-04-13T01:06:17+00:00</updated>
	<published>2021-04-13T01:06:17+00:00</published>
	<title>Citing grave threat, Scientific American replaces 'climate change' with 'climate emergency'</title>
</entry>", "worldnews", "worldnews,mprsni")]
	[InlineData(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<entry xmlns=""http://www.w3.org/2005/Atom"">
	<author>
		<name>/u/Sleepybystander</name>
		<uri>https://old.reddit.com/user/Sleepybystander</uri>
	</author>
	<category term=""worldnews"" label=""r/worldnews"" />
	<content type=""html"">&lt;!-- SC_OFF --&gt;&lt;div class=&quot;md&quot;&gt;&lt;p&gt;How about &amp;quot;War on climate&amp;quot; so they can use military budget on them?&lt;/p&gt; &lt;/div&gt;&lt;!-- SC_ON --&gt;</content>
	<id>t1_guc9zyq</id>
	<link href=""https://old.reddit.com/r/worldnews/comments/mprsni/citing_grave_threat_scientific_american_replaces/guc9zyq/""/>
	<updated>2021-04-13T05:20:06+00:00</updated>
	<title>/u/Sleepybystander on Citing grave threat, Scientific American replaces 'climate change' with 'climate emergency'</title>
</entry>", "worldnews,mprsni", "worldnews,mprsni,guc9zyq")]
	public void ProcessChildren(string xml, string tagsCsv, string expectedCsv)
	{
		var entry = xml.Deserialize<Models.Generated.entry>();
		var tags = tagsCsv.Split(',').ToList();
		var expected = expectedCsv.Split(',').ToList();

		switch (entry.id[..3])
		{
			case "t1_":
				tags.Add(entry.id[3..]);
				break;
			case "t3_":
				tags.Add(entry.id[3..]);
				break;
			default:
				Assert.True(false, entry.id + " should've started with t3_");
				break;
		}

		Assert.Equal(expected, tags);
	}

	[Theory]
	[InlineData(
		"https://old.reddit.com/r/subaru/?utm_campaign=redirect&utm_medium=desktop&utm_source=reddit&utm_name=random_subreddit",
		"subaru")]
	public void SubredditFromLocationUri(string uriString, string expected)
	{
		var uri = new Uri(uriString, UriKind.Absolute);

		var actual = Concrete.Client.SubredditFromuri(uri);
		Assert.Equal(expected, actual);
	}
}

public static class Extensions
{
	private readonly static Encoding _encoding = Encoding.UTF8;
	private readonly static XmlSerializerFactory _xmlSerializerFactory = new();

	public static T Deserialize<T>(this string xml)
		where T : class
	{
		using var stream = new MemoryStream(_encoding.GetBytes(xml));
		return _xmlSerializerFactory.CreateSerializer(typeof(T))
			.Deserialize(stream) as T
			?? throw new ArgumentOutOfRangeException($"{typeof(T).Name} could not be deserialized from {xml}");
	}
}
