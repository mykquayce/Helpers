using Helpers.SSH;
using Helpers.SSH.Concrete;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddBaseClient(this IServiceCollection services, Action<IConfig> builder)
	{
		IConfig config = new SshClientConfig();
		builder(config);
		var @base = BuildBaseClient(config);
		return services
			.AddSingleton(Options.Options.Create(config))
			.AddTransient(_ => @base);
	}

	public static IServiceCollection AddSshClient(this IServiceCollection services, Action<IConfig> builder)
	{
		return services
			.AddBaseClient(builder)
			.AddTransient<IClient, Client>()
			.AddTransient<IService, Service>();
	}

	private static Renci.SshNet.SshClient BuildBaseClient(IConfig config)
	{
		ArgumentException.ThrowIfNullOrEmpty(config.Host);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(config.Port);

		if (config.PathToPrivateKey != null)
		{
			ArgumentException.ThrowIfNullOrEmpty(config.PathToPrivateKey);
			var keyFile = new Renci.SshNet.PrivateKeyFile(config.PathToPrivateKey);
			return new(config.Host, config.Port, config.Username, keyFile);
		}

		if (config.Password != null)
		{
			ArgumentException.ThrowIfNullOrEmpty(config.Password);
			return new(config.Host, config.Port, config.Username, config.Password);
		}

		return new(config.Host, config.Port, config.Username);
	}

	private struct SshClientConfig : IConfig
	{
		public string Host { get; set; }
		public ushort Port { get; set; }
		public string Username { get; set; }
		public string? Password { get; set; }
		public string? PathToPrivateKey { get; set; }
	}
}
