using System.Text;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Models.Tests;

public class CastTests
{
	[Theory]
	[InlineData(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<entry xmlns=""http://www.w3.org/2005/Atom"">
    <author>
        <name>/u/europhilic</name>
        <uri>https://old.reddit.com/user/europhilic</uri>
    </author>
    <category term=""euphoria"" label=""r/euphoria"" />
    <content type=""html"">&lt;!-- SC_OFF --&gt;&lt;div class=&quot;md&quot;&gt;&lt;blockquote&gt; &lt;p&gt;the one where they go OOOOHHAHHH or some type of yelling?&lt;/p&gt; &lt;/blockquote&gt; &lt;p&gt;same&lt;/p&gt; &lt;/div&gt;&lt;!-- SC_ON --&gt;</content>
    <id>t1_ew061lo</id>
    <link href=""https://old.reddit.com/r/euphoria/comments/cm3ryv/euphoria_s1_e8_and_salt_the_earth_behind_you/ew061lo/""/>
    <updated>2019-08-05T03:47:51+00:00</updated>
    <title>/u/europhilic on Euphoria: S1 E8 &quot;And Salt the Earth Behind You&quot; - Post-Episode Discussion</title>
</entry>")]
	public void Comment(string xml)
	{
		var entry = Deserialize<Generated.entry>(xml);

		Assert.NotNull(entry);

		var comment = (IComment)entry!;

		Assert.NotNull(comment);
		Assert.NotNull(comment.Id);
		Assert.NotEqual("t1_ew061lo", comment.Id);
		Assert.Equal("ew061lo", comment.Id);
	}

	[Theory]
	[InlineData(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:media=""http://search.yahoo.com/mrss/"">
	<author>
		<name>/u/Beateride</name>
		<uri>https://old.reddit.com/user/Beateride</uri>
	</author>
	<category term=""xbox"" label=""r/xbox""/>
	<content type=""html"">&lt;table&gt; &lt;tr&gt;&lt;td&gt; &lt;a href=&quot;https://old.reddit.com/r/xbox/comments/izrg6u/external_drive_on_xbox_series/&quot;&gt; &lt;img src=&quot;https://b.thumbs.redditmedia.com/t0-yb8QzTk34UUkajHkwsHUk9nnt4afSEZyvRejUlcY.jpg&quot; alt=&quot;External Drive on Xbox Series&quot; title=&quot;External Drive on Xbox Series&quot; /&gt; &lt;/a&gt; &lt;/td&gt;&lt;td&gt; &amp;#32; submitted by &amp;#32; &lt;a href=&quot;https://old.reddit.com/user/Beateride&quot;&gt; /u/Beateride &lt;/a&gt; &lt;br/&gt; &lt;span&gt;&lt;a href=&quot;https://i.imgur.com/U69zbXb.jpg&quot;&gt;[link]&lt;/a&gt;&lt;/span&gt; &amp;#32; &lt;span&gt;&lt;a href=&quot;https://old.reddit.com/r/xbox/comments/izrg6u/external_drive_on_xbox_series/&quot;&gt;[comments]&lt;/a&gt;&lt;/span&gt; &lt;/td&gt;&lt;/tr&gt;&lt;/table&gt;</content>
	<id>t3_izrg6u</id>
	<media:thumbnail url=""https://b.thumbs.redditmedia.com/t0-yb8QzTk34UUkajHkwsHUk9nnt4afSEZyvRejUlcY.jpg"" />
	<link href=""https://old.reddit.com/r/xbox/comments/izrg6u/external_drive_on_xbox_series/"" />
	<updated>2020-09-25T20:34:21+00:00</updated>
	<title>External Drive on Xbox Series</title>
</entry>")]
	public void Thread(string xml)
	{
		var entry = Deserialize<Generated.entry>(xml);

		Assert.NotNull(entry);

		var thread = (IThread)entry!;

		Assert.NotNull(thread);
		Assert.NotNull(thread.Id);
		Assert.NotEqual("t3_izrg6u", thread.Id);
		Assert.Equal("izrg6u", thread.Id);
	}

	private static T Deserialize<T>(string s)
	{
		var serializer = new XmlSerializer(typeof(T));
		var bytes = Encoding.UTF8.GetBytes(s);
		using var stream = new MemoryStream(bytes);
		return (T)serializer.Deserialize(stream)!;
	}
}
