using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

		[Theory]
		[InlineData("A quick brown fox jumps over the lazy dog", 1341739415)]
		[InlineData("Hello world", -1694202214)]
		public void ExtensionMethodsTests_DeterministicHashCode(string before, int expected)
		{
			Assert.Equal(
				expected,
				before.GetDeterministicHashCode());
		}

		[Fact]
		public void GetData()
		{
			var inner = new Exception
			{
				Data =
				{
					["three"] = 3,
					["four"] = 4,
				},
			};

			var middle = new Exception(default, inner);

			var outer = new Exception(default, middle)
			{
				Data =
				{
					["one"] = 1,
					["two"] = 2,
				},
			};

			// Act
			var data = new Dictionary<object, object?>(outer.GetData());

			// Assert
			Assert.NotNull(data);
			Assert.NotEmpty(data);
			Assert.Equal(4, data!.Count);
			Assert.Contains("one", data.Keys);
			Assert.Contains("two", data.Keys);
			Assert.Contains("three", data.Keys);
			Assert.Contains("four", data.Keys);
			Assert.Equal(1, data["one"]);
			Assert.Equal(2, data["two"]);
			Assert.Equal(3, data["three"]);
			Assert.Equal(4, data["four"]);

			var enumerator = data.GetEnumerator();

			Assert.True(enumerator.MoveNext());

			Assert.Equal("one", enumerator.Current.Key);
			Assert.Equal(1, enumerator.Current.Value);
		}

		[Fact]
		public void EnumerateFileSystemInfosLeafFirstTest()
		{
			// Arrange
			var path = Path.Combine(GetClassDirectory(), "1");
			var root = new DirectoryInfo(path);

			// Act
			var directories = root.EnumerateFileSystemInfosLeafFirst().ToList();

			// Assert
			Assert.NotNull(directories);
			Assert.NotEmpty(directories);
			Assert.Equal(5, directories.Count);
			Assert.Equal("111",  directories[0].Name);
			Assert.Equal("11",   directories[1].Name);
			Assert.Equal("1211", directories[2].Name);
			Assert.Equal("121",  directories[3].Name);
			Assert.Equal("12",   directories[4].Name);
		}

		private static string GetClassDirectory([CallerFilePath] string? callerFilePath = default)
			=> Path.GetDirectoryName(callerFilePath)!;
	}
}
