using Dawn;
using System.Text.RegularExpressions;

namespace Helpers.Reddit.Models.Concrete;

public record Entry(string Id, Entry.Types Type, string Subreddit, string? Author, string Content, Uri Link, string Title, DateTime Updated)
	: IComment, IThread
{
	private readonly static Regex _idRegex = new(@"^[Tt]\d_", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	public static explicit operator Entry(Generated.entry entry)
	{
		Guard.Argument(entry).NotNull().Wrap(e => e.id)
			.NotNull().NotEmpty().NotWhiteSpace().Matches("^t[13]_[0-9a-z]{6,7}$");

		var id = entry.id[3..];
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
		var type = entry.id[1] switch
		{
			'1' => Types.Comment,
			'3' => Types.Thread,
		};
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
		var subreddit = entry.category.term;
		var author = entry.author?.name;
		var content = entry.content.Value;
		var link = new Uri(entry.link.href, UriKind.Absolute);
		var title = entry.title;
		var updated = entry.updated;

		return new Entry(id, type, subreddit, author, content, link, title, updated);
	}

	[Flags]
	public enum Types : byte
	{
		Comment = 1,
		Thread = 2,
	}
}
