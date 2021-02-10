using System.Linq;
using Xunit;

namespace Helpers.Phasmophobia
{
	public class EnumExtensionsTests
	{
		[Theory]
		[InlineData(Evidences.EmfLevelFive, Evidences.EmfLevelFive)]
		[InlineData(Evidences.EmfLevelFive | Evidences.Fingerprints, Evidences.EmfLevelFive, Evidences.Fingerprints)]
		[InlineData(Evidences.EmfLevelFive | Evidences.Fingerprints | Evidences.FreezingTemperatures, Evidences.EmfLevelFive, Evidences.Fingerprints, Evidences.FreezingTemperatures)]
		public void ArrayFromFlagsEnum(Evidences before, params Evidences[] expected)
		{
			var actual = before.GetFlags().ToList();

			Assert.Equal(expected, actual);
		}
	}
}
