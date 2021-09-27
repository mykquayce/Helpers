namespace Helpers.Reddit.Models;

public interface IEntry
{
	string Author { get; }
	string Id { get; }
	DateTime Updated { get; }
	string Title { get; }
	Uri Link { get; }
	MessageType MessageType { get; }
	string Subreddit { get; }
}
