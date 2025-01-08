using System.Net;
using System.Net.Http.Json;

namespace Helpers.GitHub.Clients.Concrete
{
	public sealed class GitHubClient(HttpClient httpClient) : IGitHubClient
	{
		public IAsyncEnumerable<Models.ForkObject> GetForksAsync(string owner, string repo)
			=> GetCollectionAsync<Models.ForkObject>($"/repos/{owner}/{repo}/forks");

		public IAsyncEnumerable<Models.BranchSummaryObject> GetBranchesAsync(string owner, string repo)
			=> GetCollectionAsync<Models.BranchSummaryObject>($"/repos/{owner}/{repo}/branches");

		public Task<Models.BranchObject> GetBranchAsync(string owner, string repo, string branch)
			=> GetAsync<Models.BranchObject>($"/repos/{owner}/{repo}/branches/{branch}");

		public ValueTask<Models.BranchObject.CommitObject> GetLastCommitForBranchAsync(string owner, string repo, string sha)
			=> GetCollectionAsync<Models.BranchObject.CommitObject>($"/repos/{owner}/{repo}/commits?per_page=1&sha={sha}")
				.SingleAsync();

		private async Task<T> GetAsync<T>(string requestUri)
			where T : class
		{
			var uri = new Uri(requestUri, UriKind.Relative);
			var response = await httpClient.GetAsync(uri);

			if (response.StatusCode == HttpStatusCode.Forbidden)
			{
				throw new Exception("Rate-limit exceeded");
			}

			var o = await response.Content.ReadFromJsonAsync<T>();
			return o;
		}

		private async IAsyncEnumerable<T> GetCollectionAsync<T>(string requestUri)
		{
			var collection = await GetAsync<T[]>(requestUri);
			foreach (var item in collection) yield return item;
		}
	}
}
