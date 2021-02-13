using System;

namespace Helpers.GitHub.Models
{
	public record BranchObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? name { get; init; }
		public CommitObject? commit { get; init; }
		public _Links? _links { get; init; }
		public bool? _protected { get; init; }
		public Protection? protection { get; init; }
		public string? protection_url { get; init; }
	}

	public record CommitObject
	{
		public string? sha { get; init; }
		public string? node_id { get; init; }
		public Commit1Object? commit { get; init; }
		public string? url { get; init; }
		public string? html_url { get; init; }
		public string? comments_url { get; init; }
		public Author1Object? author { get; init; }
		public Author1Object? committer { get; init; }
		public Parent[]? parents { get; init; }
	}

	public record Commit1Object
	{
		public AuthorObject? author { get; init; }
		public AuthorObject? committer { get; init; }
		public string? message { get; init; }
		public TreeObject? tree { get; init; }
		public string? url { get; init; }
		public int? comment_count { get; init; }
		public VerificationObject? verification { get; init; }
	}

	public record AuthorObject
	{
		public string? name { get; init; }
		public string? email { get; init; }
		public DateTime? date { get; init; }
	}

	public record TreeObject
	{
		public string? sha { get; init; }
		public string? url { get; init; }
	}

	public record VerificationObject
	{
		public bool? verified { get; init; }
		public string? reason { get; init; }
		public object? signature { get; init; }
		public object? payload { get; init; }
	}

	public record Author1Object
	{
		public string? login { get; init; }
		public int? id { get; init; }
		public string? node_id { get; init; }
		public string? avatar_url { get; init; }
		public string? gravatar_id { get; init; }
		public string? url { get; init; }
		public string? html_url { get; init; }
		public string? followers_url { get; init; }
		public string? following_url { get; init; }
		public string? gists_url { get; init; }
		public string? starred_url { get; init; }
		public string? subscriptions_url { get; init; }
		public string? organizations_url { get; init; }
		public string? repos_url { get; init; }
		public string? events_url { get; init; }
		public string? received_events_url { get; init; }
		public string? type { get; init; }
		public bool? site_admin { get; init; }
	}

	public record Parent
	{
		public string? sha { get; init; }
		public string? url { get; init; }
		public string? html_url { get; init; }
	}

	public record _Links
	{
		public string? self { get; init; }
		public string? html { get; init; }
	}

	public record Protection
	{
		public bool? enabled { get; init; }
		public Required_Status_Checks? required_status_checks { get; init; }
	}

	public record Required_Status_Checks
	{
		public string? enforcement_level { get; init; }
		public object[]? contexts { get; init; }
	}
#pragma warning restore IDE1006 // Naming Styles
}
