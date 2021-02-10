using System;
using System.Collections.Generic;

namespace Helpers.PhilipsHue.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record RuleObject(
		string? name, string? owner, DateTime? created, string? lasttriggered, int? timestriggered,
		string? status, bool? recycle, IList<RuleObject.Condition>? conditions, IList<RuleObject.Action>? actions)
	{
		public record Condition(string? address, string? @operator, string? value);

		public record Action(string? address, string? method, Action.Body? body)
		{
			public record Body(int? bri_inc, bool? on, string? scene, object? status, string? time, int? transitiontime);
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
