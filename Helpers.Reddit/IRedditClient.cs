using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Reddit
{
	public interface IRedditClient : IDisposable
	{
		IAsyncEnumerable<Models.Generated.entry> GetCommentsAsync(string subreddit, string threadId);
		Task<string> GetRandomSubredditAsync();
		IAsyncEnumerable<Models.Generated.entry> GetThreadsAsync(string subreddit);
	}
}
