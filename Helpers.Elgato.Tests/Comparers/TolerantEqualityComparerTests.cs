using Xunit;

namespace Helpers.Elgato.Tests.Comparers;

public class TolerantEqualityComparerTests
{
	[Theory]
	[InlineData(0, 0, true)]
	[InlineData(0, 1, true)]
	[InlineData(0, 2, false)]
	[InlineData(0, 3, false)]
	[InlineData(1, 0, true)]
	[InlineData(2, 0, false)]
	[InlineData(3, 0, false)]
	public void Test1(int left, int right, bool expected)
	{
		IEqualityComparer<int> sut = TolerantEqualityComparer<int>.One;

		var actual = sut.Equals(left, right);

		Assert.Equal(expected, actual);
	}
}
