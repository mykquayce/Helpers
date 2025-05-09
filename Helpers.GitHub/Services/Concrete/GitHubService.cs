﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.GitHub.Services.Concrete
{
	public sealed class GitHubService : IGitHubService
	{
		private readonly Clients.IGitHubClient _client;

		public GitHubService(Clients.IGitHubClient client)
		{
			_client = client
				?? throw new ArgumentNullException(nameof(client));
		}

		public Task<Models.BranchObject> GetBranchAsync(string owner, string repo, string branch)
			=> _client.GetBranchAsync(owner, repo, branch);

		public IAsyncEnumerable<Models.BranchSummaryObject> GetBranchesAsync(string owner, string repo)
			=> _client.GetBranchesAsync(owner, repo);

		public IAsyncEnumerable<Models.ForkObject> GetForksAsync(string owner, string repo)
			=> _client.GetForksAsync(owner, repo);

		public ValueTask<Models.BranchObject.CommitObject> GetLastCommitForBranchAsync(string owner, string repo, string sha)
			=> _client.GetLastCommitForBranchAsync(owner, repo, sha);

		public async IAsyncEnumerable<(string branch, DateTime dateTime)> GetTimeStampsFromBranchesAsync(string owner, string repo)
		{
			var branchSummaries = _client.GetBranchesAsync(owner, repo);

			await foreach (var branchSummary in branchSummaries)
			{
				var branch = await _client.GetBranchAsync(owner, repo, branchSummary.name);

				var name = branch.name ?? throw new ArgumentNullException(nameof(Models.BranchObject.name));
				var date = branch.commit?.commit?.author?.date ?? throw new ArgumentNullException(nameof(Models.BranchObject.CommitObject.Commit1Object.AuthorObject.date));

				yield return (name, date);
			}
		}
	}
}
