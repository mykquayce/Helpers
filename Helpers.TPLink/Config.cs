namespace Helpers.TPLink
{
	public record Config(ushort Port)
	{
		public const ushort DefaultPort = 9_999;

		public Config() : this(DefaultPort) { }

		public static Config Defaults => new();
	}
}
