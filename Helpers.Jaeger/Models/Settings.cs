namespace Helpers.Jaeger.Models
{
	public class Settings
	{
		public string? ServiceName { get; set; }
		public string? Host { get; set; } = "localhost";
		public int? Port { get; set; } = 6_831;
	}
}
