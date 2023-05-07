namespace Helpers.PhilipsHue.Tests.Services;

public class GroupsServiceTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IService _service;

	public GroupsServiceTests(Fixtures.Fixture fixture)
	{
		_service = fixture.Service;
	}

	[Theory]
	[InlineData("bedroom")]
	public Task GetGroupPowerTests(string alias)
	{
		return _service.GetGroupPowerAsync(alias);
	}

	[Theory]
	[InlineData("bedroom", false)]
	[InlineData("bedroom", true)]
	public Task SetGroupPowerTests(string alias, bool on)
	{
		return _service.SetGroupPowerAsync(alias, on);
	}

	[Theory]
	[InlineData("bedroom")]
	public Task ToggleGroupPowerTests(string alias)
	{
		return _service.ToggleGroupPowerAsync(alias);
	}
}
