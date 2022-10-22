using System.Linq;
using Xunit;

namespace Helpers.DockerHub.Tests;

public class RegistryClientTests : IClassFixture<Fixtures.RegistryClientFixture>
{
	private readonly IRegistryClient _sut;

	public RegistryClientTests(Fixtures.RegistryClientFixture fixture)
	{
		_sut = fixture.RegistryClient;
	}

	[Theory]
	[InlineData("pihole", "pihole")]
	public async Task GetTagsTests(string organization, string repository)
	{
		var tags = await _sut.GetTagsAsync(organization, repository).ToListAsync();

		Assert.NotNull(tags);
		Assert.NotEmpty(tags);
		Assert.DoesNotContain(default, tags);
	}
}
