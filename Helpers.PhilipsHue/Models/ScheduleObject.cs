using System;

namespace Helpers.PhilipsHue.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record ScheduleObject(
		string? name, string? description, ScheduleObject.CommandObject? command, string? time,
		DateTime? created, string? status, bool? autodelete, DateTime? starttime, bool? recycle)
	{
		public record CommandObject(string address, CommandObject.Body? body, string method)
		{
			public record Body(int? status);
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
