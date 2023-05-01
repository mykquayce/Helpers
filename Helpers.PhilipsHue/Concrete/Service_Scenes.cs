namespace Helpers.PhilipsHue.Concrete;

public partial class Service
{
	public async Task ApplySceneToGroupAsync(string scene, string group, TimeSpan transition = default, CancellationToken cancellationToken = default)
	{
		var sceneId = await ResolveSceneNameAsync(scene, cancellationToken);
		var groupIndex = await ResolveGroupNameAsync(group, cancellationToken);
		await _client.ApplySceneToGroupAsync(groupIndex, sceneId, transition, cancellationToken);
	}
}
