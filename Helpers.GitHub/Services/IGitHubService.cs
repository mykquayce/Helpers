using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.GitHub.Services
{
	public interface IGitHubService : IDisposable
	{
		Task<Models.BranchObject> GetBranchAsync(string owner, string repo, string branch);
		IAsyncEnumerable<Models.BranchSummaryObject> GetBranchesAsync(string owner, string repo);
		IAsyncEnumerable<Models.ForkObject> GetForksAsync(string owner, string repo);
		ValueTask<Models.BranchObject.CommitObject> GetLastCommitForBranchAsync(string owner, string repo, string sha);
		IAsyncEnumerable<(string branch, DateTime dateTime)> GetTimeStampsFromBranchesAsync(string owner, string repo);
	}
}
