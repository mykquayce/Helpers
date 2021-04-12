using System.IO;
using System.Text;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Models.Tests
{
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
		public void Thread(string xml)
		{
			var serializer = new XmlSerializer(typeof(Generated.entry));
			var bytes = Encoding.UTF8.GetBytes(xml);
			using var stream = new MemoryStream(bytes);

			var entry = serializer.Deserialize(stream) as Generated.entry;

			Assert.NotNull(entry);

			var thread = (IThread)(Concrete.Entry)entry!;

			Assert.NotNull(thread);
			Assert.NotNull(thread.Id);
			Assert.NotEqual("t1_ew061lo", thread.Id);
			Assert.Equal("ew061lo", thread.Id);
		}
	}
}
