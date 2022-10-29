namespace Helpers.GlobalCache.Tests;

[Collection(nameof(NonParallelCollection))]
public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
{
	private readonly IService _service;

	public ServiceTests(Fixtures.ServiceFixture fixture)
	{
		_service = fixture.Service;
	}

	[Theory]
	[InlineData("amp-mute-toggle", "amp-mute-toggle", "amp-mute-toggle", "amp-mute-toggle")]
	[InlineData("amp-mute-toggle", "amp-mute-toggle", "amp-mute-toggle")]
	[InlineData("amp-mute-toggle", "amp-mute-toggle")]
	[InlineData("amp-mute-toggle")]
	public async Task SendMessageTests(params string[] presets)
	{
		using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);
		foreach (var preset in presets)
		{
			await _service.SendMessageAsync(preset, cts.Token);
			await Task.Delay(millisecondsDelay: 1_000, cts.Token);
		}
	}
}
