using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GitHub.Tests
{
	public class ClientTests : IClassFixture<Fixtures.GitHubClientFixture>
	{
		private readonly Clients.IGitHubClient _sut;

		public ClientTests(Fixtures.GitHubClientFixture fixture) => _sut = fixture.GitHubClient;

		[Theory]
		[InlineData("octokit", "octokit.rb")]
		public async Task GetForks(string owner, string repo)
		{
			var forks = await _sut.GetForksAsync(owner, repo).ToListAsync();

			Assert.NotNull(forks);
			Assert.NotEmpty(forks);
			Assert.All(forks, Assert.NotNull);
		}

		[Theory]
		[InlineData("octokit", "octokit.rb")]
		public async Task GetBranches(string owner, string repo)
		{
			var branches = await _sut.GetBranchesAsync(owner, repo).ToListAsync();

			Assert.NotNull(branches);
			Assert.NotEmpty(branches);
			Assert.All(branches, Assert.NotNull);
			Assert.All(branches, b => Assert.NotNull(b.@protected));
		}

		[Theory]
		[InlineData("octokit", "octokit.rb", "commits")]
		public async Task GetBranch(string owner, string repo, string branch)
		{
			var details = await _sut.GetBranchAsync(owner, repo, branch);

			Assert.NotNull(details);
			Assert.Equal(branch, details.name);
		}

		[Theory]
		[InlineData("octokit", "octokit.rb", "f91c900991eeca999d8532637308690d2844c6df")]
		public async Task GetLastCommitForBranch(string owner, string repo, string sha)
		{
			var commit = await _sut.GetLastCommitForBranchAsync(owner, repo, sha);

			Assert.NotNull(commit);
		}
	}
}
