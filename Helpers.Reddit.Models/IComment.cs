namespace Helpers.Reddit.Models;

public interface IComment : IEntry
{
	string Content { get; }
}
