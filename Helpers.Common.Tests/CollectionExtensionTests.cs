using Xunit;

namespace Helpers.Common.Tests;

public class CollectionExtensionTests
{
	[Theory]
	[InlineData(new[] { 1, }, new[] { 1, })]
	[InlineData(new[] { 1, 2, }, new[] { 1, 2, })]
	[InlineData(new[] { 1, 2, }, new[] { 1, })]
	[InlineData(new[] { 1, 2, 3, }, new[] { 1, 2, 3, })]
	[InlineData(new[] { 1, 2, 3, 4, }, new[] { 1, 2, 3, 4, })]
	public void DoubleEnumerator_GetEnumerator(int[] left, int[] right)
	{
		Assert.False(ReferenceEquals(left, right));

		var tuple = (left, right);
		using var enumerator = tuple.GetEnumerator();

		while (enumerator.MoveNext())
		{
			var (l, r) = enumerator.Current;

			Assert.Equal(l, r);
		}
	}

	[Theory]
	[InlineData(new[] { 1, }, new[] { 1, })]
	[InlineData(new[] { 1, 2, }, new[] { 1, 2, })]
	[InlineData(new[] { 1, 2, }, new[] { 1, })]
	[InlineData(new[] { 1, 2, 3, }, new[] { 1, 2, 3, })]
	[InlineData(new[] { 1, 2, 3, 4, }, new[] { 1, 2, 3, 4, })]
	public void DoubleEnumerator_ForEach(int[] left, int[] right)
	{
		Assert.False(ReferenceEquals(left, right));

		var tuple = (left, right);

		foreach (var (l, r) in tuple)
		{
			Assert.Equal(l, r);
		}
	}

	[Theory]
	[InlineData(.5d, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, }, new[] { 0, 2, 4, 6, 8, })]
	[InlineData(.25d, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, }, new[] { 0, 4, 8, })]
	public void TakeSampleTests(double ratio, int[] before, int[] expected)
	{
		var actual = before.TakeSample(ratio).ToList();

		Assert.Equal(expected, actual);
	}
}
