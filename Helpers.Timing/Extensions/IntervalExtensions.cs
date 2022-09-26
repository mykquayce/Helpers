namespace Helpers.Timing;

public static class IntervalExtensions
{
	internal static Func<DateTime> GetUtcNow { get; set; } = () => DateTime.UtcNow;

	public static DateTime Next(this IInterval interval) => interval.GetUpcoming().First();
	public static DateTime Previous(this IInterval interval) => interval.GetExpired().First();

	public static IEnumerable<DateTime> GetExpired(this IInterval interval)
	{
		var now = GetUtcNow();
		var scale = interval.GetScale();
		var index = (long)Math.Floor(now.Ticks / scale);
		while (true)
		{
			var ticks = (long)(index * scale);
			yield return new(ticks, DateTimeKind.Utc);
			index--;
		}
	}

	public static IEnumerable<DateTime> GetUpcoming(this IInterval interval)
	{
		var now = GetUtcNow();
		var scale = interval.GetScale();
		var index = (long)Math.Ceiling(now.Ticks / scale);
		while (true)
		{
			var ticks = (long)(index * scale);
			yield return new(ticks, DateTimeKind.Utc);
			index++;
		}
	}

	private static double GetScale(this IInterval interval)
		=> interval.Count * interval.Unit.GetTicks();

	public static Task SleepTillNextAsync(this IInterval interval, CancellationToken? cancellationToken = null)
	{
		var delay = interval.Next() - GetUtcNow();
		var millisecondInterval = (int)delay.TotalMilliseconds;
		return Task.Delay(millisecondInterval, cancellationToken ?? CancellationToken.None);
	}
}
