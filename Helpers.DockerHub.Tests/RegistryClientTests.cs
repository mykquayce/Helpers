using Xunit;

namespace Helpers.DockerHub.Tests;

public class RegistryClientTests : IClassFixture<Fixtures.RegistryClientFixture>
{
	private readonly IRegistryClient _sut;

	public RegistryClientTests(Fixtures.RegistryClientFixture fixture)
	{
		_sut = fixture.RegistryClient;
	}

	[Fact]
	public async Task GetTagsTests()
	{
		var tags = await _sut.GetTagsAsync().ToListAsync();

		Assert.NotNull(tags);
		Assert.NotEmpty(tags);
		Assert.DoesNotContain(default, tags);
	}
}
