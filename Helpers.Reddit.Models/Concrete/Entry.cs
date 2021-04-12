using System;
using System.Text.RegularExpressions;

namespace Helpers.Reddit.Models.Concrete
{
	public record Entry(string Id, string Subreddit, string? Author, string Content, Uri Link, string Title, DateTime Updated)
		: IComment, IThread
	{
		private readonly static Regex _idRegex = new(@"^[Tt]\d_", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static explicit operator Entry(Generated.entry entry)
		{
			var id = _idRegex.IsMatch(entry.id) ? entry.id[3..] : entry.id;
			var subreddit = entry.category.term;
			var author = entry.author?.name;
			var content = entry.content.Value;
			var link = new Uri(entry.link.href, UriKind.Absolute);
			var title = entry.title;
			var updated = entry.updated;

			return new Entry(id, subreddit, author, content, link, title, updated);
		}
	}
}
