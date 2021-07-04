using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.GitHub.Clients
{
	public interface IGitHubClient : IDisposable
	{
		Task<Models.BranchObject> GetBranchAsync(string owner, string repo, string branch);
		IAsyncEnumerable<Models.BranchSummaryObject> GetBranchesAsync(string owner, string repo);
		IAsyncEnumerable<Models.ForkObject> GetForksAsync(string owner, string repo);
		ValueTask<Models.BranchObject.CommitObject> GetLastCommitForBranchAsync(string owner, string repo, string sha);
	}
}
