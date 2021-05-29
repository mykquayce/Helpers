using System;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.TPLink.Models.Generated
{
#pragma warning disable IDE1006 // Naming Styles
	public record ResponseObject(ResponseObject.SystemObject system, ResponseObject.EmeterObject? emeter)
	{
		public record SystemObject(SystemObject.SystemInfoObject get_sysinfo)
		{
			public record SystemInfoObject(
				string sw_ver,
				Version hw_ver,
				string model,
				string deviceId,
				[property: JsonConverter(typeof(Helpers.Json.Converters.JsonGuidConverter))] Guid oemId,
				[property: JsonConverter(typeof(Helpers.Json.Converters.JsonGuidConverter))] Guid hwId,
				int rssi,
				int latitude_i,
				int longitude_i,
				string alias,
				string status,
				string obd_src,
				string mic_type,
				string feature,
				[property: JsonConverter(typeof(Helpers.Json.Converters.JsonPhysicalAddressConverter))] PhysicalAddress mac,
				int updating,
				int led_off,
				int relay_state,
				int on_time,
				string icon_hash,
				string dev_name,
				string active_mode,
				SystemInfoObject.NextActionObject next_action,
				int ntc_state,
				Enums.ErrorCode err_code)
			{
				public record NextActionObject(int type);
			}
		}

		public record EmeterObject(EmeterObject.RealtimeDataObject get_realtime)
		{
			public record RealtimeDataObject(int current_ma, int voltage_mv, int power_mw, int total_wh, Enums.ErrorCode err_code);
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
