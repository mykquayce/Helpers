using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Helpers.PhilipsHue.Concrete;

public partial class Client
{
	public Task ApplySceneToGroupAsync(int group, string scene, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var requestUri = $"{_uriPrefix}/groups/{group:D}/action";
		var body = new GroupAction(scene, transition);
		return this.PutAsJsonAsync(requestUri, body, cancellationToken);
	}

	public async IAsyncEnumerable<KeyValuePair<string, int>> GetGroupsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestUri = _uriPrefix + "/groups";
		var dictionary = await this.GetFromJsonAsync<Dictionary<int, Group>>(requestUri, cancellationToken);
		foreach (var (id, group) in dictionary)
		{
			yield return new(group.name, id);
		}
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct Group(string name);

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct GroupAction(string scene, int transitiontime)
	{
		public GroupAction(string scene, TimeSpan transition)
			: this(scene, (int)(transition.TotalMilliseconds) / 100)
		{ }
	}
}
