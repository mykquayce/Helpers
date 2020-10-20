namespace Helpers.TPLink.Models
{
	public class ResponseDataObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public SystemObject? system { get; init; }
		public EmeterObject? emeter { get; init; }

		public class SystemObject
		{
			public SysInfoObject? get_sysinfo { get; init; }

			public class SysInfoObject
			{
				public string? sw_ver { get; init; }
				public string? hw_ver { get; init; }
				public string? model { get; init; }
				public string? deviceId { get; init; }
				public string? oemId { get; init; }
				public string? hwId { get; init; }
				public int? rssi { get; init; }
				public int? longitude_i { get; init; }
				public int? latitude_i { get; init; }
				public string? alias { get; init; }
				public string? status { get; init; }
				public string? mic_type { get; init; }
				public string? feature { get; init; }
				public string? mac { get; init; }
				public int? updating { get; init; }
				public int? led_off { get; init; }
				public int? relay_state { get; init; }
				public int? on_time { get; init; }
				public string? active_mode { get; init; }
				public string? icon_hash { get; init; }
				public string? dev_name { get; init; }
				public NextActionObject? next_action { get; init; }
				public int? ntc_state { get; init; }
				public int? err_code { get; init; }

				public class NextActionObject
				{
					public int? type { get; init; }
				}
			}
		}

		public class EmeterObject
		{
			public RealtimeObject? get_realtime { get; init; }

			public class RealtimeObject
			{
				public int? voltage_mv { get; init; }
				public int? current_ma { get; init; }
				public int? power_mw { get; init; }
				public int? total_wh { get; init; }
				public int? err_code { get; init; }
			}
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
