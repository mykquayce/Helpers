using System;

namespace Helpers.GitHub.Tests.Fixtures
{
	public class GitHubClientFixture
	{
		public GitHubClientFixture()
		{
			var fixture = new HttpClientFixture();

			GitHubClient = new Clients.Concrete.GitHubClient(fixture.HttpClient);
		}

		public Clients.IGitHubClient GitHubClient { get; }
	}
}
