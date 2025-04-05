using Cronos;

namespace Helpers.Timing.Tests;

public class CronTests
{
	private static readonly DateTimeOffset _now = DateTimeOffset.Parse("2025-02-09T11:13:21.0540598Z");

	[Theory]
	[InlineData("0 */3 * * *", "2025-02-09T12:00:00Z")]
	[InlineData("0 */6 * * *", "2025-02-09T12:00:00Z")]
	[InlineData("0 */8 * * *", "2025-02-09T16:00:00Z")]
	public void Test1(string s, string expected)
	{
		var ok = CronExpression.TryParse(s, CronFormat.Standard, out var expession);

		Assert.True(ok);

		var actual = expession.GetNextOccurrence(_now, TimeZoneInfo.Utc);

		Assert.NotNull(actual);
		Assert.Equal(DateTimeOffset.Parse(expected), actual.Value);
	}
}
