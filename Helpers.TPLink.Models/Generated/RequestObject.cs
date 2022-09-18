namespace Helpers.TPLink.Models.Generated;

#pragma warning disable IDE1006 // Naming Styles
public record RequestObject(RequestObject.SystemObject system, RequestObject.EmeterObject? emeter)
{
	public RequestObject(SystemObject system) : this(system, null) { }

	public record SystemObject(
		SystemObject.SetRelayStateObject? set_relay_state,
		SystemObject.GetSysInfoObject? get_sysinfo)
	{
		public SystemObject(SetRelayStateObject set_relay_state) : this(set_relay_state, null) { }
		public SystemObject(GetSysInfoObject get_sysinfo) : this(null, get_sysinfo) { }

		public record SetRelayStateObject(int state);
		public record GetSysInfoObject();
	}

	public record EmeterObject(EmeterObject.GetRealtimeObject get_realtime)
	{
		public record GetRealtimeObject();
	}
}
#pragma warning restore IDE1006 // Naming Styles
