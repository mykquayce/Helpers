using System;
using System.Collections.Generic;
using Xunit;

namespace Helpers.Common.Tests
{
	public class ExtensionMethodsTests
	{
		[Theory]
		[InlineData(default, "")]
		[InlineData("", "")]
		[InlineData("a", "a")]
		[InlineData(
			"frtpfrtpahesnehnfrtpaskenfharltp,sku.enlhufitr,pasfnlhateurswp,ftanlrshwep,unrtpsfntrpnisfeufpiluhtfprsulihftprsiulhen",
			"frtpfrtpahesnehnfrtpaskenfharltp,sku.enlhufitr,pasfnlhateurswp,ftanlrshwep,unrtpsfntrpnisfeufpiluhtf")]
		public void ExtensionMethodsTests_Truncate(string before, string expected)
		{
			Assert.Equal(
				expected,
				before.Truncate());
		}

		[Fact]
		public void ExtensionMethodsTests_ToKeyValuePairString()
		{
			// Arrange
			var before = new Dictionary<string, int>
			{
				["one"] = 1,
				["two"] = 2,
				["three"] = 3,
			};

			// Act
			var actual = before.ToKeyValuePairString();

			// Assert
			Assert.Equal("one=1;two=2;three=3;", actual);
		}

		[Fact]
		public void ExtensionMethodsTests_AddRange()
		{
			// Arrange
			var first = new Dictionary<string, int>
			{
				["one"] = 1,
				["two"] = 2,
				["three"] = 3,
			};

			var second = new Dictionary<string, int>
			{
				["four"] = 4,
				["five"] = 5,
				["six"] = 6,
			};

			// Act
			first.AddRange(second);

			// Assert
			Assert.Equal(6, first.Count);
			Assert.Equal(1, first["one"]);
			Assert.Equal(2, first["two"]);
			Assert.Equal(3, first["three"]);
			Assert.Equal(4, first["four"]);
			Assert.Equal(5, first["five"]);
			Assert.Equal(6, first["six"]);
		}

		[Theory]
		[InlineData(
			"https://old.reddit.com/r/nostalgia/?utm_campaign=redirect&utm_medium=desktop&utm_source=reddit&utm_name=random_subreddit",
			"https://old.reddit.com/r/nostalgia/")]
		public void ExtensionMethodsTests_StripQuery(string uriString, string expectedUriString)
		{
			var before = new Uri(uriString, UriKind.Absolute);

			var actual = before.StripQuery();

			Assert.Equal(expectedUriString, actual.OriginalString);
		}
	}
}
