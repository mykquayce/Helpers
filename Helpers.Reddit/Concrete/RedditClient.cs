using Dawn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Helpers.Reddit.Concrete
{
	public class RedditClient : IRedditClient
	{
		private readonly HttpClient _httpClient;
		private readonly XmlSerializerFactory _xmlSerializerFactory;

		public RedditClient(HttpClient httpClient, XmlSerializerFactory xmlSerializerFactory)
		{
			Guard.Argument(httpClient).NotNull().Wrap(c => c.BaseAddress!)
				.NotNull().Wrap(u => u.OriginalString)
				.NotNull().NotEmpty().NotWhiteSpace();

			_httpClient = httpClient;
			_xmlSerializerFactory = Guard.Argument(xmlSerializerFactory).NotNull().Value;
		}

		public async Task<string> GetRandomSubredditAsync()
		{
			using var request = new HttpRequestMessage(HttpMethod.Head, "/r/random");
			using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			var location = Guard.Argument(response.Headers.Location!).NotNull().Value;
			return SubredditFromuri(location!);
		}

		public static string SubredditFromuri(Uri uri)
		{
			Guard.Argument(uri).NotNull().Wrap(u => u.LocalPath)
				.NotNull().NotEmpty().NotWhiteSpace().Matches(@"^\/r\/[0-9A-Z_a-z]{2,}\/$");

			return uri.LocalPath[3..^1];
		}

		public IAsyncEnumerable<Models.Generated.entry> GetThreadsAsync(string subreddit)
		{
			Guard.Argument(subreddit).IsSubredditName();

			var uri = new Uri($"/r/{subreddit}/.rss", UriKind.Relative);

			return GetEntriesAsync(uri);
		}

		public IAsyncEnumerable<Models.Generated.entry> GetCommentsAsync(string subreddit, string threadId)
		{
			Guard.Argument(subreddit).IsSubredditName();
			Guard.Argument(threadId).IsThreadId();

			var uri = new Uri($"/r/{subreddit}/comments/{threadId}/.rss", UriKind.Relative);

			return GetEntriesAsync(uri);
		}

		private async IAsyncEnumerable<Models.Generated.entry> GetEntriesAsync(Uri uri)
		{
			await using var stream = await _httpClient.GetStreamAsync(uri);

			var feed = Deserialize<Models.Generated.feed>(stream);

			foreach (var entry in feed!.entry)
			{
				yield return entry;
			}
		}

		private T? Deserialize<T>(Stream stream) where T : class
			=> _xmlSerializerFactory.CreateSerializer(typeof(T)).Deserialize(stream) as T;

		#region dispose
		private bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_httpClient.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion dispose
	}
}
