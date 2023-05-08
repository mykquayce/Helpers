using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Helpers.PhilipsHue.Concrete;

public partial class Client
{

	public async IAsyncEnumerable<KeyValuePair<string, int>> GetGroupsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestUri = _uriPrefix + "/groups";
		var dictionary = await this.GetFromJsonAsync<Dictionary<int, Group>>(requestUri, cancellationToken);
		foreach (var (id, group) in dictionary)
		{
			yield return new(group.name, id);
		}
	}

	public async Task<bool> GetGroupPowerAsync(int group, CancellationToken cancellationToken = default)
	{
		var requestUri = $"{_uriPrefix}/groups/{group:D}";
		var action = await this.GetFromJsonAsync<GroupGetStateAction>(requestUri, cancellationToken);
		return action.state.any_on;
	}

	public Task SetGroupPowerAsync(int group, bool on, TimeSpan transition, CancellationToken cancellationToken = default)
	{
		var requestUri = $"{_uriPrefix}/groups/{group:D}/action";
		var body = new GroupSetStateAction(on, transition);
		return this.PutAsJsonAsync(requestUri, body, cancellationToken);
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct Group(string name);

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct GroupGetStateAction(GroupGetStateAction.State state)
	{
		public readonly record struct State(bool all_on, bool any_on);
	}

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct GroupSetStateAction(bool on, int transitiontime)
	{
		public GroupSetStateAction(bool on, TimeSpan transition)
			: this(on, (int)(transition.TotalMilliseconds) / 100)
		{ }
	}
}
