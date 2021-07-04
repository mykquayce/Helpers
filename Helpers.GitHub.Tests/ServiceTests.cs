using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GitHub.Tests
{
	public class ServiceTests : IClassFixture<Fixtures.GitHubServiceFixture>
	{
		private readonly Services.IGitHubService _sut;

		public ServiceTests(Fixtures.GitHubServiceFixture fixture)
		{
			_sut = fixture.GitHubService;
		}

		[Theory]
		[InlineData("octokit", "octokit.rb")]
		public async Task GetBranches(string owner, string repo)
		{
			var count = 0;
			var now = DateTime.UtcNow;
			var enumerable = _sut.GetTimeStampsFromBranchesAsync(owner, repo);

			await foreach (var (name, date) in enumerable)
			{
				count++;
				Assert.NotNull(name);
				Assert.NotEmpty(name);
				Assert.InRange(date, DateTime.UnixEpoch, now);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		[Theory]
		[InlineData("plasticrake", "tplink-smarthome-api")]
		public async Task GetTimeStampsFromBranchesFromForks(string owner, string repo)
		{
			var list = new List<(string, string, string, DateTime)>();

			try
			{
				await foreach (var fork in _sut.GetForksAsync(owner, repo))
				{
					await foreach (var branchSummary in _sut.GetBranchesAsync(fork.owner.login, fork.name))
					{
						var branch = await _sut.GetBranchAsync(fork.owner.login, fork.name, branchSummary.name);

						var tuple = (fork.owner.login, fork.name, branch.name, branch.commit.commit.author.date);

						list.Add(tuple);
					}
				}
			}
			catch { }

			var cutoff = DateTime.UtcNow.AddDays(-14);

			var filtered = list.Where(t => t.Item4 > cutoff).ToList();
		}
	}
}
