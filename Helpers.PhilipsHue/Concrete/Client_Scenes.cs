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

	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
	private readonly record struct Scene(string name);
}
