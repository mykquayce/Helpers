namespace Helpers.PhilipsHue;

public partial interface IService
{
	Task ApplySceneToGroupAsync(string scene, string group, TimeSpan transition = default, CancellationToken cancellationToken = default);
}
