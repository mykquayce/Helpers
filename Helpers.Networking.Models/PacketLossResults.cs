using System.Net.NetworkInformation;

namespace Helpers.Networking.Models;

public record PacketLossResults(DateTime DateTime, int Count, int FailedCount, double PacketLossPercentage, double AverageRoundtripTime, double AverageJitter)
{
	public PacketLossResults(DateTime dateTime, ICollection<PingReply> pingResults)
		: this(dateTime, new PingReplyCollection(pingResults))
	{ }

	public PacketLossResults(DateTime dateTime, PingReplyCollection pingReplies)
		: this(dateTime, pingReplies.Count, pingReplies.FailedCount, pingReplies.PacketLossPercentage, pingReplies.AverageRoundtripTime, pingReplies.AverageJitter)
	{ }
}
