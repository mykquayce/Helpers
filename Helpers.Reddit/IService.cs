using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Reddit
{
	public interface IService : IDisposable
	{
		Task<string> GetRandomSubredditAsync();
		IAsyncEnumerable<Models.IThread> GetThreadsAsync(string subreddit);
		IAsyncEnumerable<Models.IComment> GetCommentsAsync(string subreddit);
		IAsyncEnumerable<Models.IComment> GetCommentsAsync(Models.IThread thread);
		IAsyncEnumerable<Uri> GetUrisAsync(string subreddit);
		IAsyncEnumerable<Uri> GetUrisAsync(Models.IThread thread);
		IEnumerable<Uri> GetUris(Models.IComment comment);
	}
}
