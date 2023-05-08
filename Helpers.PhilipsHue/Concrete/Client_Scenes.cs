using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Helpers.PhilipsHue.Concrete;

public partial class Client : IClient
{
	public async IAsyncEnumerable<KeyValuePair<string, string>> GetScenesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestUri = _uriPrefix + "/scenes";
		var dictionary = await this.GetFromJsonAsync<Dictionary<string, Scene>>(requestUri, cancellationToken);
		foreach (var (id, scene) in dictionary)
		{
			yield return new(scene.name, id);
		}
	}

	public Task ApplySceneToGroupAsync(int group, string scene, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var requestUri = $"{_uriPrefix}/groups/{group:D}/action";
		var body = new GroupSceneAction(scene, transition);
		return this.PutAsJsonAsync(requestUri, body, cancellationToken);
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct Scene(string name);

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct GroupSceneAction(string scene, int transitiontime)
	{
		public GroupSceneAction(string scene, TimeSpan transition)
			: this(scene, (int)(transition.TotalMilliseconds) / 100)
		{ }
	}
}
