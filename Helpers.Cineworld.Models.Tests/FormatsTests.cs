using Xunit;

namespace Helpers.Cineworld.Models.Tests
{
	public class FormatsTests
	{
		[Theory]
		[InlineData(Enums.Formats._2d | Enums.Formats.Imax, Enums.Formats.Imax)]
		[InlineData(Enums.Formats._2d, Enums.Formats.None)]
		[InlineData(Enums.Formats._3d | Enums.Formats.Imax, Enums.Formats._3d | Enums.Formats.Imax)]
		[InlineData(Enums.Formats._3d, Enums.Formats._3d)]
		public void FormatsTests_Strip2d(Enums.Formats before, Enums.Formats expected)
		{
			var actual = before & ~Enums.Formats._2d;

			Assert.Equal(expected, actual);
		}
	}
}
