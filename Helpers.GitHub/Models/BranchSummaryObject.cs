namespace Helpers.GitHub.Models
{
	public record BranchSummaryObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? name { get; init; }
		public CommitObject? commit { get; init; }
		public bool? @protected { get; init; }

		public record CommitObject
		{
			public string? sha { get; init; }
			public string? url { get; init; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
