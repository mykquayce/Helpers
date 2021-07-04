using System;

namespace Helpers.GitHub.Models
{
#pragma warning disable CS8618, IDE1006 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable., Naming Styles
	public record ForkObject
	{
		public bool _private { get; init; }
		public string archive_url { get; init; }
		public bool archived { get; init; }
		public string assignees_url { get; init; }
		public string blobs_url { get; init; }
		public string branches_url { get; init; }
		public string clone_url { get; init; }
		public string collaborators_url { get; init; }
		public string comments_url { get; init; }
		public string commits_url { get; init; }
		public string compare_url { get; init; }
		public string contents_url { get; init; }
		public string contributors_url { get; init; }
		public DateTime created_at { get; init; }
		public string default_branch { get; init; }
		public string deployments_url { get; init; }
		public string description { get; init; }
		public bool disabled { get; init; }
		public string downloads_url { get; init; }
		public string events_url { get; init; }
		public bool fork { get; init; }
		public int forks { get; init; }
		public int forks_count { get; init; }
		public string forks_url { get; init; }
		public string full_name { get; init; }
		public string git_commits_url { get; init; }
		public string git_refs_url { get; init; }
		public string git_tags_url { get; init; }
		public string git_url { get; init; }
		public bool has_downloads { get; init; }
		public bool has_issues { get; init; }
		public bool has_pages { get; init; }
		public bool has_projects { get; init; }
		public bool has_wiki { get; init; }
		public string homepage { get; init; }
		public string hooks_url { get; init; }
		public string html_url { get; init; }
		public int id { get; init; }
		public string issue_comment_url { get; init; }
		public string issue_events_url { get; init; }
		public string issues_url { get; init; }
		public string keys_url { get; init; }
		public string labels_url { get; init; }
		public string language { get; init; }
		public string languages_url { get; init; }
		public ForkObject.LicenseObject license { get; init; }
		public string merges_url { get; init; }
		public string milestones_url { get; init; }
		public object mirror_url { get; init; }
		public string name { get; init; }
		public string node_id { get; init; }
		public string notifications_url { get; init; }
		public int open_issues { get; init; }
		public int open_issues_count { get; init; }
		public ForkObject.OwnerObject owner { get; init; }
		public string pulls_url { get; init; }
		public DateTime pushed_at { get; init; }
		public string releases_url { get; init; }
		public int size { get; init; }
		public string ssh_url { get; init; }
		public int stargazers_count { get; init; }
		public string stargazers_url { get; init; }
		public string statuses_url { get; init; }
		public string subscribers_url { get; init; }
		public string subscription_url { get; init; }
		public string svn_url { get; init; }
		public string tags_url { get; init; }
		public string teams_url { get; init; }
		public string trees_url { get; init; }
		public DateTime updated_at { get; init; }
		public string url { get; init; }
		public int watchers { get; init; }
		public int watchers_count { get; init; }

		public record OwnerObject
		(
			string avatar_url,
			string events_url,
			string followers_url,
			string following_url,
			string gists_url,
			string gravatar_id,
			string html_url,
			int id,
			string login,
			string node_id,
			string organizations_url,
			string received_events_url,
			string repos_url,
			bool site_admin,
			string starred_url,
			string subscriptions_url,
			string type,
			string url
		);

		public record LicenseObject(string key, string name, string node_id, string spdx_id, string url);
	}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
