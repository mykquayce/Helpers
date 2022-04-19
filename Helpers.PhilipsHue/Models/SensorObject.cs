namespace Helpers.PhilipsHue.Models;

#pragma warning disable IDE1006 // Naming Styles
public record SensorObject(
	SensorObject.StateObject state, SensorObject.ConfigObject config, string name, string type, string modelid,
	string manufacturername, string swversion, SensorObject.SoftwareUpdate swupdate, string productname,
	string diversityid, string uniqueid, SensorObject.Capabilities capabilities, bool recycle)
{
	public record StateObject(object? daylight, string? lastupdated, int? buttonevent, int? status);
	public record ConfigObject(
		bool? on, bool? configured, int? sunriseoffset, int? sunsetoffset,
		int? battery, bool? reachable, IList<object>? pending);

	public record SoftwareUpdate(string? state, DateTime? lastinstall);

	public record Capabilities(bool? certified, bool? primary, IList<Capabilities.InputObject>? inputs)
	{
		public record InputObject(IList<int>? repeatintervals, IList<EventObject>? events);
	}

	public record EventObject(int? buttonevent, string? eventtype);
}
#pragma warning restore IDE1006 // Naming Styles
