using System.Net.NetworkInformation;

namespace Helpers.Networking.Models;

public record PingReplyCollection(ICollection<PingReply> PingReplies)
{
	public int Count { get; } = PingReplies.Count;
	public int FailedCount { get; } = PingReplies.Count(r => r.Status != IPStatus.Success);
	public IReadOnlyCollection<long> SuccessfulRoundtripTimes { get; } = PingReplies.Where(r => r.Status == IPStatus.Success).Select(r => r.RoundtripTime).ToList();
	public double PacketLossPercentage => (FailedCount * 100d) / PingReplies.Count;
	public double AverageRoundtripTime => SuccessfulRoundtripTimes.Average();
	public IReadOnlyCollection<double> Jitter => SuccessfulRoundtripTimes.Select(i => Math.Abs(i - AverageRoundtripTime)).ToList();
	public double AverageJitter => Jitter.Average();
}
