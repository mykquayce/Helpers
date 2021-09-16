﻿using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Networking.Clients.Concrete
{
	public class PingClient : IPingClient
	{
		public record Config(int Timeout)
		{
			public const int DefaultTimeout = 1_000;

			public Config() : this(DefaultTimeout) { }

			public static Config Defaults => new(DefaultTimeout);
		}

		private readonly int _timeout;

		public PingClient(IOptions<Config> options)
		{
			_timeout = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Wrap(c => c.Timeout).Positive().Value;
		}

		public async Task<Models.PacketLossResults> PacketLossTestAsync(IPAddress ip, int milliseconds = 10_000)
		{
			var now = DateTime.UtcNow;
			var results = await PingsAsync(ip, milliseconds).ToListAsync();
			return new(now, results);
		}

		public Task<PingReply> PingAsync(IPAddress ip) => PingsAsync(ip, _timeout).FirstAsync().AsTask();

		public async IAsyncEnumerable<PingReply> PingsAsync(IPAddress ip, int milliseconds)
		{
			Guard.Argument(milliseconds).Positive();

			using var cts = new CancellationTokenSource(milliseconds);

			await foreach (var reply in PingsAsync(ip, cts.Token))
			{
				yield return reply;
			}
		}

		public async IAsyncEnumerable<PingReply> PingsAsync(IPAddress ip, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			Guard.Argument(ip).NotNull();
			Guard.Argument(cancellationToken).NotDefault();

			using var ping = new Ping();

			while (!cancellationToken.IsCancellationRequested)
			{
				yield return await ping.SendPingAsync(ip, _timeout);
			}
		}
	}
}
