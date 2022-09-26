using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Helpers.Timing.Tests;

public sealed class IntervalExtensionsTests : IDisposable
{
	private readonly Func<DateTime> _previousGetUtcNow;

	public IntervalExtensionsTests()
	{
		_previousGetUtcNow = IntervalExtensions.GetUtcNow;
		IntervalExtensions.GetUtcNow = () => new DateTime(2022, 9, 25, 22, 11, 19, DateTimeKind.Utc);
	}

	public void Dispose()
	{
		IntervalExtensions.GetUtcNow = _previousGetUtcNow;
	}

	[Theory]
	[InlineData(Units.Day, .25, "2022-09-26T00:00:00Z", "2022-09-26T06:00:00Z", "2022-09-26T12:00:00Z")]
	[InlineData(Units.Day, .5, "2022-09-26T00:00:00Z", "2022-09-26T12:00:00Z", "2022-09-27T00:00:00Z")]
	[InlineData(Units.Day, 1, "2022-09-26T00:00:00Z", "2022-09-27T00:00:00Z", "2022-09-28T00:00:00Z")]
	[InlineData(Units.Hour, .25, "2022-09-25T22:15:00Z", "2022-09-25T22:30:00Z", "2022-09-25T22:45:00Z")]
	[InlineData(Units.Hour, .5, "2022-09-25T22:30:00Z", "2022-09-25T23:00:00Z", "2022-09-25T23:30:00Z")]
	[InlineData(Units.Hour, 1, "2022-09-25T23:00:00Z", "2022-09-26T00:00:00Z", "2022-09-26T01:00:00Z")]
	[InlineData(Units.Hour, 2, "2022-09-26T00:00:00Z", "2022-09-26T02:00:00Z", "2022-09-26T04:00:00Z")]
	[InlineData(Units.Hour, 4, "2022-09-26T00:00:00Z", "2022-09-26T04:00:00Z", "2022-09-26T08:00:00Z")]
	public void NextTests(Units unit, double count, params string[] expected)
	{
		IInterval interval = new Interval(unit, count);

		var actual = interval.GetUpcoming().Take(expected.Length).ToArray();

		Assert.Equal(expected.Select(Parse), actual);
	}

	[Theory]
	[InlineData(Units.Day, .25, "2022-09-25T18:00:00Z", "2022-09-25T12:00:00Z", "2022-09-25T06:00:00Z")]
	[InlineData(Units.Day, .5, "2022-09-25T12:00:00Z", "2022-09-25T00:00:00Z", "2022-09-24T12:00:00Z")]
	[InlineData(Units.Day, 1, "2022-09-25T00:00:00Z", "2022-09-24T00:00:00Z", "2022-09-23T00:00:00Z")]
	[InlineData(Units.Hour, .25, "2022-09-25T22:00:00Z", "2022-09-25T21:45:00Z", "2022-09-25T21:30:00Z")]
	[InlineData(Units.Hour, .5, "2022-09-25T22:00:00Z", "2022-09-25T21:30:00Z", "2022-09-25T21:00:00Z")]
	[InlineData(Units.Hour, 1, "2022-09-25T22:00:00Z", "2022-09-25T21:00:00Z", "2022-09-25T20:00:00Z")]
	[InlineData(Units.Hour, 2, "2022-09-25T22:00:00Z", "2022-09-25T20:00:00Z", "2022-09-25T18:00:00Z")]
	[InlineData(Units.Hour, 4, "2022-09-25T20:00:00Z", "2022-09-25T16:00:00Z", "2022-09-25T12:00:00Z")]
	public void PreviousTests(Units unit, double count, params string[] expected)
	{
		IInterval interval = new Interval(unit, count);

		var actual = interval.GetExpired().Take(expected.Length).ToArray();

		Assert.Equal(expected.Select(Parse), actual);
	}

	[Theory(Skip = "runs long (41 seconds)")]
	[InlineData(Units.Minute, 4, 41_000)]
	[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "runs long (41 seconds)")]
	public async Task SleepTests(Units unit, double count, int expected)
	{
		IInterval interval = new Interval(unit, count);

		var stopwatch = Stopwatch.StartNew();
		await interval.SleepTillNextAsync();
		stopwatch.Stop();

		Assert.Equal(expected, stopwatch.ElapsedMilliseconds, tolerance: 1_000d);
	}

	private static DateTime Parse(string s) => DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
}
