using Xunit;

namespace Helpers.Jaeger.Tests
{
	public class ExtensionsTests
	{
		[Theory]
		[InlineData(-1, 'a')]
		[InlineData(-0, 'a')]
		[InlineData(.000000001, 'b')]
		[InlineData(.5, 'b')]
		[InlineData(.999999999, 'b')]
		[InlineData(1, 'c')]
		[InlineData(2, 'c')]
		public void PatternMatchingTest(double input, char expected)
		{
			var actual = input switch
			{
				double d when d <= 0 => 'a',
				double d when d >= 1 => 'c',
				_ => 'b',
			};

			Assert.Equal(expected, actual);
		}
	}
}
