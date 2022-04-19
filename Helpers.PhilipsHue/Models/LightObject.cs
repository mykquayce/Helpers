namespace Helpers.PhilipsHue.Models;

#pragma warning disable IDE1006 // Naming Styles
public record LightObject(
	LightObject.StateObject? state, LightObject.SoftwareUpdateObject? swupdate, string? type, string? name,
	string? modelid, string? manufacturername, string? productname, LightObject.CapabilitiesObject? capabilities,
	LightObject.ConfigObject? config, string? uniqueid, string? swversion, string? swconfigid, string? productid)
{
	public record StateObject(
		bool? on, byte? bri, ushort? hue, byte? sat, string? effect, IList<float>? xy,
		int? ct, string? alert, string? colormode, string? mode, bool? reachable)
	{
		public static StateObject Off => new(false, default, default, default, default, default, default, default, default, default, default);
		public static StateObject On => new(true, default, default, default, default, default, default, default, default, default, default);
		public static StateObject Bright => new(default, (byte)(.6f * byte.MaxValue), default, default, default, default, default, default, default, default, default);
		public static StateObject Brighter => new(default, (byte)(.8f * byte.MaxValue), default, default, default, default, default, default, default, default, default);
		public static StateObject Brightest => new(default, byte.MaxValue, default, default, default, default, default, default, default, default, default);
		public static StateObject Red => new(default, default, default, default, default, new[] { .66f, .3f, }, default, default, "xy", default, default);
		public static StateObject Green => new(default, default, default, default, default, new[] { .2f, .64f, }, default, default, "xy", default, default);
		public static StateObject Blue => new(default, default, default, default, default, new[] { .2f, .1f, }, default, default, "xy", default, default);
		public static StateObject Warmest => new(default, default, default, default, default, default, 500, default, "ct", default, default);
		public static StateObject Warm => new(default, default, default, default, default, default, 450, default, "ct", default, default);
		public static StateObject Cold => new(default, default, default, default, default, default, 200, default, "ct", default, default);
		public static StateObject Coldest => new(default, default, default, default, default, default, 150, default, "ct", default, default);

		public static StateObject operator +(StateObject left, StateObject right)
		{
			return new StateObject(
				left.on ?? right.on,
				left.bri ?? right.bri,
				left.hue ?? right.hue,
				left.sat ?? right.sat,
				left.effect ?? right.effect,
				left.xy ?? right.xy,
				left.ct ?? right.ct,
				left.alert ?? right.alert,
				left.colormode ?? right.colormode,
				left.mode ?? right.mode,
				left.reachable ?? right.reachable);
		}
	}

	public record SoftwareUpdateObject(string? state, DateTime? lastinstall);

	public record CapabilitiesObject(bool? certified, CapabilitiesObject.ControlObject? control, CapabilitiesObject.StreamingObject? streaming)
	{
		public record ControlObject(int? mindimlevel, int? maxlumen, string? colorgamuttype, IList<IList<float>>? colorgamut, ControlObject.CtObject? ct)
		{
			public record CtObject(int? min, int? max);
		}

		public record StreamingObject(bool? renderer, bool? proxy);
	}

	public record ConfigObject(string? archetype, string? function, string? direction, ConfigObject.StartupObject? startup)
	{
		public record StartupObject(string? mode, bool? configured);
	}
}
#pragma warning restore IDE1006 // Naming Styles
