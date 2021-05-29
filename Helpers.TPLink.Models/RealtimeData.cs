namespace Helpers.TPLink.Models
{
	public record RealtimeData(double Amps, double Volts, double Watts)
	{
		public static explicit operator RealtimeData(Generated.ResponseObject.EmeterObject.RealtimeDataObject generated)
			=> new(generated.current_ma / 1_000d, generated.voltage_mv / 1_000d, generated.power_mw / 1_000d);
	}
}
