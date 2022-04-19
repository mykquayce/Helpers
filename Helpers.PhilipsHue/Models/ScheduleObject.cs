namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record ScheduleObject(
	string name, string description, ScheduleObject.CommandObject command, string time,
	DateTime created, string status, bool autodelete, DateTime starttime, bool recycle)
{
	public record CommandObject(string address, CommandObject.Body body, string method)
	{
		public record Body(int status);
	}
}
