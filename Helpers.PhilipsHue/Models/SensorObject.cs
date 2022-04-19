namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record SensorObject(
	SensorObject.StateObject state, SensorObject.ConfigObject config, string name, string type, string modelid,
	string manufacturername, string swversion, SensorObject.SoftwareUpdate swupdate, string productname,
	string diversityid, string uniqueid, SensorObject.Capabilities capabilities, bool recycle)
{
	public record StateObject(object daylight, string lastupdated, int buttonevent, int status);
	public record ConfigObject(
		bool on, bool configured, int sunriseoffset, int sunsetoffset,
		int battery, bool reachable, IReadOnlyList<object> pending);

	public record SoftwareUpdate(string state, DateTime lastinstall);

	public record Capabilities(bool certified, bool primary, IReadOnlyList<Capabilities.InputObject> inputs)
	{
		public record InputObject(IReadOnlyList<int> repeatintervals, IReadOnlyList<EventObject> events);
	}

	public record EventObject(int buttonevent, string eventtype);
}
