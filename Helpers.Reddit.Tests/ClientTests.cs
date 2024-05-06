using Helpers.Reddit.Models.Generated;
using System.Diagnostics.CodeAnalysis;

namespace Helpers.Reddit.Tests;

public partial class ClientTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IClient _sut = fixture.Client;

	[Fact]
	public async Task GetRandomSubredditTests()
	{
		var subreddit = await _sut.GetRandomSubredditAsync();

		Assert.NotEmpty(subreddit);
	}

	[Theory, InlineData("worldnews", 100)]
	public async Task GetThreadsTests(string subredditName, int count)
	{
		// Act
		var entries = await _sut.GetThreadsAsync(subredditName).Take(count).ToArrayAsync();

		// Assert
		Assert.NotEmpty(entries);
		Assert.Equal(count, entries.Length);
		Assert.DoesNotContain(null, entries);
		Assert.Distinct(entries, EntryComparer.Instance);
	}

	private class EntryComparer : IEqualityComparer<entryType>
	{
		public bool Equals(entryType? x, entryType? y) => x?.id == y?.id;
		public int GetHashCode([DisallowNull] entryType obj) => obj.id.GetHashCode();

		public static EntryComparer Instance { get; } = new EntryComparer();
	}
}

