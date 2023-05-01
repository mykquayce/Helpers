namespace Helpers.PhilipsHue.Tests.Clients;

public class SceneClientTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IClient _sut;

	public SceneClientTests(Fixtures.Fixture fixture)
	{
		_sut = fixture.Client;
	}

	[Fact]
	public async Task GetSceneTests()
	{
		// Act
		var dictionary = await _sut.GetScenesAsync()
			.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

		// Assert
		Assert.NotNull(dictionary);
		Assert.NotEmpty(dictionary);
		Assert.DoesNotContain(string.Empty, dictionary.Keys);
		Assert.DoesNotContain(default, dictionary.Values);
	}
}
