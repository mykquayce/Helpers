using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Helpers.GitHub.Tests
{
	public class SerializationTests
	{
		[Theory]
		[InlineData(".", "Data", "forks.json")]
		public void Forks(params string[] paths)
		{
			ICollection<Models.ForkObject> forks;
			{
				var path = Path.Combine(paths);
				var json = File.ReadAllText(path);
				forks = JsonSerializer.Deserialize<Models.ForkObject[]>(json)
					?? throw new Exception();
			}
			Assert.NotEmpty(forks);
		}
		[Theory]
		[InlineData(".", "Data", "branches.json")]
		public void Branches(params string[] paths)
		{
			ICollection<Models.BranchObject> branches;
			{
				var path = Path.Combine(paths);
				var json = File.ReadAllText(path);
				branches = JsonSerializer.Deserialize<Models.BranchObject[]>(json)
					?? throw new Exception();
			}
			Assert.NotEmpty(branches);
			Assert.DoesNotContain(default, branches);
		}

		[Theory]
		[InlineData(".", "Data", "branch.json")]
		public void Branch(params string[] paths)
		{
			Models.BranchSummaryObject branch;
			{
				var path = Path.Combine(paths);
				var json = File.ReadAllText(path);
				branch = JsonSerializer.Deserialize<Models.BranchSummaryObject>(json)
					?? throw new Exception();
			}
			Assert.NotNull(branch);
		}

		[Theory]
		[InlineData(".", "Data", "lastcommit.json")]
		public void LastCommit(params string[] paths)
		{
			ICollection<Models.BranchObject.CommitObject> commits;
			{
				var path = Path.Combine(paths);
				var json = File.ReadAllText(path);
				commits = JsonSerializer.Deserialize<Models.BranchObject.CommitObject[]>(json)
					?? throw new Exception();
			}
			Assert.NotNull(commits);
			Assert.DoesNotContain(default, commits);
			Assert.Single(commits);
		}
	}
}
