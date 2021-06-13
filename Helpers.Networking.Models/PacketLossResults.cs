using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Helpers.Networking.Models
{
	public record PacketLossResults(ICollection<PingReply> PingResults) : IReadOnlyCollection<PingReply>
	{
		public int FailedCount { get; } = PingResults.Count(r => r.Status != IPStatus.Success);
		public IReadOnlyCollection<long> SuccessfulRoundtripTimes { get; } = PingResults.Where(r => r.Status == IPStatus.Success).Select(r => r.RoundtripTime).ToList();
		public double PacketLossPercentage => (FailedCount * 100d) / PingResults.Count;
		public double Average => SuccessfulRoundtripTimes.Average();
		public IReadOnlyCollection<double> Jitter => SuccessfulRoundtripTimes.Select(i => Math.Abs(i - Average)).ToList();
		public double AverageJitter => Jitter.Average();

		#region IReadOnlyCollection implementation
		public int Count { get; } = PingResults.Count;
		public IEnumerator<PingReply> GetEnumerator() => PingResults.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion IReadOnlyCollection implementation

		public override string ToString() => string.Join(Environment.NewLine, ToStrings());

		public IEnumerable<string> ToStrings()
		{
			var type = this.GetType();
			var properties = type.GetProperties();

			foreach (var property in from p in properties
									 where p.CanRead
									 orderby p.Name
									 select p)
			{
				var value = property.GetValue(this);

				if (value is not IEnumerable)
				{
					yield return $"{property.Name}={value}";
				}
			}
		}
	}
}
