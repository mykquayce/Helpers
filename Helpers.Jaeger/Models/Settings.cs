namespace Helpers.Jaeger.Models
{
	public record Settings(string? ServiceName, string Host, ushort Port, double SamplingRate)
	{
		public const string DefaultHost = "localhost";
		public const ushort DefaultPort = 6_831;
		public const double DefaultSamplingRate = 1d;
	}
}
