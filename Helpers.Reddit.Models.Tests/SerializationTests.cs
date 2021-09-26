using System.Text;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Models.Tests;

public class SerializationTests
{
	[Theory]
	[InlineData("worldnews.xml")]
	[InlineData("thread1.xml")]
	[InlineData("thread2.xml")]
	public async Task Deserialize_HasAllTheValues(string fileName)
	{
		var path = Path.Combine(".", "Data", fileName);

		await using var stream = File.OpenRead(path);

		var factory = new XmlSerializerFactory();

		var serializer = factory.CreateSerializer(typeof(Generated.feed));

		var feed = serializer.Deserialize(stream) as Generated.feed;

		Assert.NotNull(feed);
		AssertNotMalformedString(feed!.title);
		Assert.NotNull(feed.entry);
		Assert.NotEmpty(feed.entry);

		foreach (var entry in feed.entry)
		{
			Assert.NotNull(entry);
			Assert.NotNull(entry.author);
			AssertNotMalformedString(entry.author.name);
			Assert.NotNull(entry.content);
			AssertNotMalformedString(entry.content.Value);
			Assert.NotNull(entry.link);
			AssertNotMalformedString(entry.link.href);
			Assert.Null(entry.link.Value);
			Assert.NotEqual(default, entry.updated);
			Assert.InRange(entry.updated, DateTime.MinValue, DateTime.UtcNow);
			AssertNotMalformedString(entry.title);
		}
	}

	private static void AssertNotMalformedString(string? s)
	{
		Assert.NotNull(s);
		Assert.NotEmpty(s);
		Assert.DoesNotMatch(@"^\s+$", s);
		Assert.DoesNotMatch(@"^\s", s);
		Assert.DoesNotMatch(@"\s$", s);
	}

	[Theory]
	[InlineData(
		"<?xml version=\"1.0\" encoding=\"UTF-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><author><name>/u/Grpc96</name><uri>https://old.reddit.com/user/Grpc96</uri></author><category term=\"worldnews\" label=\"r/worldnews\"/><content type=\"html\">&amp;#32; submitted by &amp;#32; &lt;a href=&quot;https://old.reddit.com/user/Grpc96&quot;&gt; /u/Grpc96 &lt;/a&gt; &lt;br/&gt; &lt;span&gt;&lt;a href=&quot;https://www.esquiremag.ph/politics/news/china-backs-down-a00293-20210416-lfrm&quot;&gt;[link]&lt;/a&gt;&lt;/span&gt; &amp;#32; &lt;span&gt;&lt;a href=&quot;https://old.reddit.com/r/worldnews/comments/ms54d0/china_backs_away_as_philippines_and_us_send/&quot;&gt;[comments]&lt;/a&gt;&lt;/span&gt;</content><id>t3_ms54d0</id><link href=\"https://old.reddit.com/r/worldnews/comments/ms54d0/china_backs_away_as_philippines_and_us_send/\" /><updated>2021-04-16T15:07:27+00:00</updated><published>2021-04-16T15:07:27+00:00</published><title>China Backs Away as Philippines and U.S. Send Impressive Fleet to West Philippine Sea</title></entry>",
		Concrete.Entry.Types.Thread)]
	[InlineData(
		"<?xml version=\"1.0\" encoding=\"UTF-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><author><name>/u/Smart_Resist615</name><uri>https://old.reddit.com/user/Smart_Resist615</uri></author><category term=\"worldnews\" label=\"r/worldnews\" /><content type=\"html\">&lt;!-- SC_OFF --&gt;&lt;div class=&quot;md&quot;&gt;&lt;p&gt;Wow she dropped a blatant dog whistle doing a TV spot at fucking holocaust museum. How dumb can you be?&lt;/p&gt; &lt;/div&gt;&lt;!-- SC_ON --&gt;</content><id>t1_guq40ia</id><link href=\"https://old.reddit.com/r/worldnews/comments/ms2jkn/3_resign_from_auschwitz_museum_board_over/guq40ia/\"/><updated>2021-04-16T14:00:25+00:00</updated><title>/u/Smart_Resist615 on 3 resign from Auschwitz museum board over appointment of right-wing politician</title></entry>",
		Concrete.Entry.Types.Comment)]
	public void SerializeDeserialize(string xml, Concrete.Entry.Types expected)
	{
		var serializer = new XmlSerializer(typeof(Generated.entry));

		var bytes = Encoding.UTF8.GetBytes(xml);
		using var stream = new MemoryStream(bytes);

		var generated = serializer.Deserialize(stream) as Generated.entry;

		Assert.NotNull(generated);

		var concrete = (Concrete.Entry)generated!;

		Assert.NotNull(concrete);
		Assert.Equal(expected, concrete.Type);
	}
}
