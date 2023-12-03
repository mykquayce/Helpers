﻿namespace Helpers.PhilipsHue.Tests.Clients;

public class GroupsClientTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IClient _sut;

	public GroupsClientTests(Fixtures.Fixture fixture)
	{
		_sut = fixture.Client;
	}

	[Theory]
	[InlineData(1, "cYkDQhEYyWJIMIn", 0)]
	[InlineData(1, "M00iBpzqC1oqhW0", 1_000)]
	public Task ApplySceneToGroupTests(int group, string scene, int transition)
	{
		return _sut.ApplySceneToGroupAsync(group, scene, TimeSpan.FromMilliseconds(transition));
	}

	[Fact]
	public async Task GetGroupsTests()
	{
		// Act
		var dictionary = await _sut.GetGroupsAsync()
			.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

		// Assert
		Assert.NotNull(dictionary);
		Assert.NotEmpty(dictionary);
		Assert.DoesNotContain(string.Empty, dictionary.Keys);
		Assert.DoesNotContain(default, dictionary.Values);
	}

	[Theory]
	[InlineData(1, false)]
	[InlineData(1, true)]
	public Task SetGroupPowerTests(int group, bool on)
	{
		return _sut.SetGroupPowerAsync(group, on);
	}

	[Theory]
	[InlineData(1)]
	public Task ToggleGroupPower(int group)
	{
		return _sut.ToggleGroupPowerAsync(group);
	}

	[Theory]
	[InlineData(1)]
	public Task GetGroupPower(int group)
	{
		return _sut.GetGroupPowerAsync(group);
	}
}