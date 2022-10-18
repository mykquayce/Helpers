namespace Helpers.Timing;

public static class UnitsExtensions
{
	public static long GetTicks(this Units unit)
	{
		return unit switch
		{
			Units.Day => TimeSpan.TicksPerDay,
			Units.Hour => TimeSpan.TicksPerHour,
			Units.Millisecond => TimeSpan.TicksPerMillisecond,
			Units.Minute => TimeSpan.TicksPerMinute,
			Units.Second => TimeSpan.TicksPerSecond,
			_ => throw new ArgumentOutOfRangeException(nameof(unit), unit, $"unexpected {nameof(unit)} value: {unit}"),
		};
	}
}
