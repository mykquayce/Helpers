using Xunit;

namespace Helpers.Common.Tests;

public class IntExtensionsTests
{
	[Theory]
	[InlineData(1, 1, 1)]
	[InlineData(2, 1, 1, 2)]
	[InlineData(3, 1, 1, 2, 3)]
	[InlineData(4, 1, 1, 2, 3)]
	[InlineData(5, 1, 1, 2, 3, 5)]
	public void GetFibanacci(int value, params int[] expected)
	{
		var actual = value.GetFibanacci().ToList();

		Assert.NotEmpty(actual);
		Assert.Equal(expected, actual);
	}
}

public static class IntExtensions
{
	/// <summary>
	/// Get Fibanacci numbers, up to and including the value of <paramref name="max"/>.
	/// </summary>
	/// <param name="max">The maximum value to return</param>
	/// <returns></returns>
	public static IEnumerable<int> GetFibanacci(this int max)
	{
		if (max < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(max), max, nameof(max) + " must be 1 or greater.")
			{
				Data = { [nameof(max)] = max, },
			};
		}

		int left = 1, right = 1;
		yield return left;

		do
		{
			yield return right;
			var i = right;
			right += left;
			left = i;
		}
		while (right <= max);
	}
}
