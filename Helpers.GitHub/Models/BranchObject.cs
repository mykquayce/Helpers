using System;

namespace Helpers.GitHub.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record BranchObject
	(
		BranchObject.Links _links,
		bool _protected,
		BranchObject.CommitObject commit,
		string name,
		BranchObject.Protection protection,
		string protection_url
	)
	{

		public record CommitObject
		(
			CommitObject.Author1Object author,
			string comments_url,
			CommitObject.Commit1Object commit,
			CommitObject.Author1Object committer,
			string node_id,
			string html_url,
			CommitObject.Parent[] parents,
			string sha,
			string url
		)
		{
			public record Author1Object
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

			public record Commit1Object
			(
				Commit1Object.AuthorObject author,
				int comment_count,
				Commit1Object.AuthorObject committer,
				string message,
				Commit1Object.TreeObject tree,
				string url,
				Commit1Object.VerificationObject verification)
			{
				public record AuthorObject(DateTime date, string email, string name);
				public record TreeObject(string sha, string url);
				public record VerificationObject(object payload, string reason, object signature, bool verified);
			}


			public record Parent(string html_url, string sha, string url);
		}

		public record Links(string html, string self);


		public record Protection(bool enabled, Protection.Required_Status_Checks required_status_checks)
		{
			public record Required_Status_Checks(object[] contexts ,string enforcement_level);
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
