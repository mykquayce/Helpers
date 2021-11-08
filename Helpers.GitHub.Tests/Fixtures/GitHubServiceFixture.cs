using System;

namespace Helpers.GitHub.Tests.Fixtures
{
	public class GitHubServiceFixture
	{
		public GitHubServiceFixture()
		{
			var fixture = new GitHubClientFixture();

			GitHubService = new Services.Concrete.GitHubService(fixture.GitHubClient);
		}

		public Services.IGitHubService GitHubService { get; }
	}
}
