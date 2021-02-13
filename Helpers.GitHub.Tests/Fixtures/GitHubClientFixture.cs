using System;

namespace Helpers.GitHub.Tests.Fixtures
{
	public sealed class GitHubClientFixture : IDisposable
	{
		public GitHubClientFixture()
		{
			var fixture = new HttpClientFixture();

			GitHubClient = new Clients.Concrete.GitHubClient(fixture.HttpClient);
		}

		public void Dispose() => GitHubClient?.Dispose();

		public Clients.IGitHubClient GitHubClient { get; }
	}
}
