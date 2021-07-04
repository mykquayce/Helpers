namespace Helpers.GitHub.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record BranchSummaryObject(string name, BranchSummaryObject.CommitObject commit, bool @protected)
	{
		public record CommitObject(string sha, string url);
	}
#pragma warning restore IDE1006 // Naming Styles
}
