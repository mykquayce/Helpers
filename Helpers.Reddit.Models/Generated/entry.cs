namespace Helpers.Reddit.Models.Generated;

public partial class entry : IComment, IThread
{
	public string Author => author.name;
	public string Id => id[3..];
	public DateTime Updated => updated;
	public string Title => title;
	public Uri Link => new(link.href, UriKind.Absolute);
	public MessageType MessageType => Enum.Parse<MessageType>(id[1].ToString());
	public string Content => content.Value;
	public string Subreddit => category.term;
}
