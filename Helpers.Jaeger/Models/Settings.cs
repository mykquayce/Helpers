﻿namespace Helpers.Jaeger.Models
{
	public class Settings
	{
		public static string DefaultHost => "localhost";
		public static int DefaultPort = 6_831;
		public static double DefaultSamplingRate = 1f;

		public string? ServiceName { get; set; }
		public string Host { get; set; } = DefaultHost;
		public int Port { get; set; } = DefaultPort;
		public double SamplingRate { get; set; } = DefaultSamplingRate;
	}
}
