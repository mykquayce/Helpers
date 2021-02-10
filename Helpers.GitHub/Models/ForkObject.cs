using System;

namespace Helpers.GitHub.Models
{
	public record ForkObject
	{
#pragma warning disable IDE1006 // Naming Stylespublic int ? id { get; init; }
		public string? node_id { get; init; }
		public string? name { get; init; }
		public string? full_name { get; init; }
		public bool? _private { get; init; }
		public OwnerObject? owner { get; init; }
		public string? html_url { get; init; }
		public string? description { get; init; }
		public bool? fork { get; init; }
		public string? url { get; init; }
		public string? forks_url { get; init; }
		public string? keys_url { get; init; }
		public string? collaborators_url { get; init; }
		public string? teams_url { get; init; }
		public string? hooks_url { get; init; }
		public string? issue_events_url { get; init; }
		public string? events_url { get; init; }
		public string? assignees_url { get; init; }
		public string? branches_url { get; init; }
		public string? tags_url { get; init; }
		public string? blobs_url { get; init; }
		public string? git_tags_url { get; init; }
		public string? git_refs_url { get; init; }
		public string? trees_url { get; init; }
		public string? statuses_url { get; init; }
		public string? languages_url { get; init; }
		public string? stargazers_url { get; init; }
		public string? contributors_url { get; init; }
		public string? subscribers_url { get; init; }
		public string? subscription_url { get; init; }
		public string? commits_url { get; init; }
		public string? git_commits_url { get; init; }
		public string? comments_url { get; init; }
		public string? issue_comment_url { get; init; }
		public string? contents_url { get; init; }
		public string? compare_url { get; init; }
		public string? merges_url { get; init; }
		public string? archive_url { get; init; }
		public string? downloads_url { get; init; }
		public string? issues_url { get; init; }
		public string? pulls_url { get; init; }
		public string? milestones_url { get; init; }
		public string? notifications_url { get; init; }
		public string? labels_url { get; init; }
		public string? releases_url { get; init; }
		public string? deployments_url { get; init; }
		public DateTime? created_at { get; init; }
		public DateTime? updated_at { get; init; }
		public DateTime? pushed_at { get; init; }
		public string? git_url { get; init; }
		public string? ssh_url { get; init; }
		public string? clone_url { get; init; }
		public string? svn_url { get; init; }
		public string? homepage { get; init; }
		public int? size { get; init; }
		public int? stargazers_count { get; init; }
		public int? watchers_count { get; init; }
		public string? language { get; init; }
		public bool? has_issues { get; init; }
		public bool? has_projects { get; init; }
		public bool? has_downloads { get; init; }
		public bool? has_wiki { get; init; }
		public bool? has_pages { get; init; }
		public int? forks_count { get; init; }
		public object? mirror_url { get; init; }
		public bool? archived { get; init; }
		public bool? disabled { get; init; }
		public int? open_issues_count { get; init; }
		public LicenseObject? license { get; init; }
		public int? forks { get; init; }
		public int? open_issues { get; init; }
		public int? watchers { get; init; }
		public string? default_branch { get; init; }

		public record OwnerObject
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
			public bool site_admin { get; init; }
		}

		public record LicenseObject
		{
			public string? key { get; init; }
			public string? name { get; init; }
			public string? spdx_id { get; init; }
			public string? url { get; init; }
			public string? node_id { get; init; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
