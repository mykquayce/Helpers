using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers.SSH.Services.Concrete
{
	public class SSHService : ISSHService
	{
		public record Config(string? Host, int? Port, string? Username, string? Password)
		{
			public Config() : this(default, default, default, default) { }
		}

		public SSHService(IOptions<Config> options)
			: this(options.Value)
		{ }

		public SSHService(Config? config)
			: this(config?.Host, config?.Port, config?.Username, config?.Password)
		{ }

		private readonly Renci.SshNet.SshClient _sshClient;

		public SSHService(string? host, int? port, string? username, string? password)
		{
			Guard.Argument(() => host!).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => port).NotNull().InRange(0, 65_535);
			Guard.Argument(() => username!).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => password!).NotNull().NotEmpty().NotWhiteSpace();

			_sshClient = new Renci.SshNet.SshClient(host, port!.Value, username, password);
		}

		private string? _newline;
		public string Newline => _newline ??= GetNewline().GetAwaiter().GetResult();

		public async Task<string> RunCommandAsync(string commandText, int millisecondsTimeout = 5_000)
		{
			Guard.Argument(() => commandText).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => millisecondsTimeout).Positive();

			if (!_sshClient.IsConnected) _sshClient.Connect();

			using var command = _sshClient.CreateCommand(commandText);

			command.CommandTimeout = TimeSpan.FromMilliseconds(millisecondsTimeout);

			return await Task.Factory.FromAsync(
				beginMethod: (callback, state) => command.BeginExecute(callback, state),
				endMethod: result => command.EndExecute(result),
				state: commandText);
		}

		#region blackhole
		public async IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync()
		{
			var commands = new[]
			{
				"ip route show | grep ^[Bb]lackhole | awk '{print($2)}'",
				"ip -6 route show | grep ^[Bb]lackhole | awk '{print($2)}'",
			};

			foreach (var command in commands)
			{
				var response = await RunCommandAsync(command);

				Guard.Argument(() => response).NotNull();

				var lines = response.Split(new char[2] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var line in lines)
				{
					var subnetAddress = Helpers.Networking.Models.SubnetAddress.Parse(line);
					yield return subnetAddress;
				}
			}
		}

		public Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress)
		{
			Guard.Argument(() => subnetAddress).NotNull();
			return RunCommandAsync("ip route add blackhole " + subnetAddress);
		}

		public Task AddBlackholesAsync(IEnumerable<Networking.Models.SubnetAddress> subnetAddresses)
			=> Task.WhenAll(subnetAddresses.Select(AddBlackholeAsync));

		public Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress)
		{
			Guard.Argument(() => subnetAddress).NotNull();
			return RunCommandAsync("ip route delete blackhole " + subnetAddress);
		}

		public Task DeleteBlackholesAsec(IEnumerable<Networking.Models.SubnetAddress> subnetAddresses)
			=> Task.WhenAll(subnetAddresses.Select(DeleteBlackholeAsync));
		#endregion blackhole

		public async IAsyncEnumerable<Helpers.Networking.Models.DhcpEntry> GetDhcpLeasesAsync()
		{
			var output = await RunCommandAsync("cat /tmp/dhcp.leases");

			var lines = output.Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				yield return Helpers.Networking.Models.DhcpEntry.Parse(line);
			}
		}

		public Task<string> GetNewline() => RunCommandAsync("echo");

		#region Dispose pattern
		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_sshClient?.IsConnected ?? false) _sshClient.Disconnect();
					_sshClient?.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion Dispose pattern
	}
}
