using System;

namespace Helpers.GitHub.Tests.Fixtures
{
	public sealed class GitHubServiceFixture : IDisposable
	{
		private readonly GitHubClientFixture _fixture;

		public GitHubServiceFixture()
		{
			_fixture = new GitHubClientFixture();

			GitHubService = new Services.Concrete.GitHubService(_fixture.GitHubClient);
		}

		public void Dispose()
		{
			GitHubService?.Dispose();
			_fixture?.Dispose();
		}

		public Services.IGitHubService GitHubService { get; }
	}
}
