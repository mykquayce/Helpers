namespace Helpers.Reddit.Models;

public enum EntryType : byte
{
	None = 0,
	Comment = 1,
	Account = 2,
	Link = 3,
	Message = 4,
	Subreddit = 5,
	Award = 6,
}
