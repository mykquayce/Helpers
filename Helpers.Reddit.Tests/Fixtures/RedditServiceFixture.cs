using System;

namespace Helpers.Reddit.Tests.Fixtures
{
	public sealed class RedditServiceFixture : IDisposable
	{
		public RedditServiceFixture()
		{
			var clientFixture = new RedditClientFixture();
			var client = clientFixture.RedditClient;
			RedditService = new Concrete.RedditService(client);
		}

		public Helpers.Reddit.IRedditService RedditService { get; }

		public void Dispose() => RedditService.Dispose();
	}
}
