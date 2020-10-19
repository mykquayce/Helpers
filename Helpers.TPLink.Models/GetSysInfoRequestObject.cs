using System.Collections.Generic;

namespace Helpers.TPLink.Models
{
	public class GetSysInfoRequestObject
	{
		public string? method { get; } = "passthrough";
		public IDictionary<string, string>? @params { get; init; }
	}
}
