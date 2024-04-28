using Helpers.Nanoleaf.Models;
using System.Text.Json;

namespace Helpers.Nanoleaf.Tests;

public class DeserializationTests
{
	[Theory, InlineData("Data", "info.json")]
	public async Task InfoTests(params string[] paths)
	{
		await using var stream = new FileStream(path: Path.Combine(paths), FileMode.Open);
		var response = await JsonSerializer.DeserializeAsync<InfoResponse>(stream);
		Assert.NotEqual(default, response);
		Assert.NotEqual(default, response!.effects);
		Assert.NotNull(response.effects.effectsList);
		Assert.NotEmpty(response.effects.effectsList);
	}
}
