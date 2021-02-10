using System.Collections.Generic;

namespace Helpers.PhilipsHue.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record ResourceLinkObject(
		string? name, string? description, string? type,
		int? classid, string? owner, bool? recycle, IList<string>? links);
#pragma warning restore IDE1006 // Naming Styles

}
