namespace Helpers.PhilipsHue;

public partial interface IClient
{
	IAsyncEnumerable<KeyValuePair<string, string>> GetScenesAsync(CancellationToken cancellationToken = default);

	Task ApplySceneToGroupAsync(int group, string scene, CancellationToken cancellationToken = default)
		=> ApplySceneToGroupAsync(group, scene, TimeSpan.Zero, cancellationToken);
	Task ApplySceneToGroupAsync(int group, string scene, TimeSpan transition, CancellationToken cancellationToken = default);
}
