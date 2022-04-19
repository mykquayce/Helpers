namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record LightObject(
	LightObject.StateObject state, LightObject.SoftwareUpdateObject swupdate, string type, string name,
	string modelid, string manufacturername, string productname, LightObject.CapabilitiesObject capabilities,
	LightObject.ConfigObject config, string uniqueid, string swversion, string swconfigid, string productid)
{
	public record StateObject(
		bool on, byte bri, ushort hue, byte sat, string effect, IReadOnlyList<float> xy,
		int ct, string alert, string colormode, string mode, bool reachable)
	{
		public StateObject Off() => this with { on = false, };
		public StateObject On() => this with { on = true, };
		public StateObject Bright() => this with { bri = (byte)(.6f * byte.MaxValue), };
		public StateObject Brighter() => this with { bri = (byte)(.8f * byte.MaxValue), };
		public StateObject Brightest() => this with { bri = byte.MaxValue, };
		public StateObject Red() => this with { xy = new[] { .66f, .3f, }, };
		public StateObject Green() => this with { xy = new[] { .2f, .64f, }, };
		public StateObject Blue() => this with { xy = new[] { .2f, .1f, }, };
		public StateObject Warmest() => this with { ct = 500, };
		public StateObject Warm() => this with { ct = 450, };
		public StateObject Cold() => this with { ct = 200, };
		public StateObject Coldest() => this with { ct = 150, };
	}

	public record SoftwareUpdateObject(string state, DateTime lastinstall);

	public record CapabilitiesObject(bool certified, CapabilitiesObject.ControlObject control, CapabilitiesObject.StreamingObject streaming)
	{
		public record ControlObject(int mindimlevel, int maxlumen, string colorgamuttype, IReadOnlyList<IReadOnlyList<float>> colorgamut, ControlObject.CtObject ct)
		{
			public record CtObject(int min, int max);
		}

		public record StreamingObject(bool renderer, bool proxy);
	}

	public record ConfigObject(string archetype, string function, string direction, ConfigObject.StartupObject startup)
	{
		public record StartupObject(string mode, bool configured);
	}
}
