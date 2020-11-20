using Dawn;
using System;
using Xunit;

namespace Helpers.DawnGuard.Tests
{
	public class StringTests
	{
		[Theory]
		[InlineData("test", false)]
		[InlineData(" test", true)]
		public void LeadingWhiteSpace(string s, bool expected)
		{
			var actual = Test(s, sut => sut.LeadingWhiteSpace());

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("test", true)]
		[InlineData(" test", false)]
		public void NoLeadingWhiteSpace(string s, bool expected)
		{
			var actual = Test(s, sut => sut.NoLeadingWhiteSpace());

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("test", false)]
		[InlineData("test ", true)]
		public void TrailingWhiteSpace(string s, bool expected)
		{
			var actual = Test(s, sut => sut.TrailingWhiteSpace());

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("test", true)]
		[InlineData("test ", false)]
		public void NoTrailingWhiteSpace(string s, bool expected)
		{
			var actual = Test(s, sut => sut.NoTrailingWhiteSpace());

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("test", false)]
		[InlineData("test ", false)]
		[InlineData(" test", false)]
		[InlineData(" test ", true)]
		public void LeadingAndTrailingWhiteSpace(string s, bool expected)
		{
			var actual = Test(s, sut => sut.LeadingAndTrailingWhiteSpace());

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("test", true)]
		[InlineData("test ", false)]
		[InlineData(" test", false)]
		[InlineData(" test ", false)]
		public void NoLeadingOrTrailingWhiteSpace(string s, bool expected)
		{
			var actual = Test(s, sut => sut.NoLeadingOrTrailingWhiteSpace());

			Assert.Equal(expected, actual);
		}

		private static bool? Test<T>(T obj, Func<Guard.ArgumentInfo<T>, Guard.ArgumentInfo<T>> f)
		{
			var sut = Guard.Argument(() => obj);

			try
			{
				f.Invoke(sut);
				return true;
			}
			catch (ArgumentException ex)
				when (ex.Source?.Equals("Dawn.Guard", StringComparison.InvariantCulture) ?? false)
			{
				return false;
			}
		}
	}
}
