using System.Linq;
using Xunit;

namespace Helpers.Phasmophobia
{
	public class UnitTest1
	{
		private const Evidences EmfLevelFive = Evidences.EmfLevelFive;
		private const Evidences Fingerprints = Evidences.Fingerprints;
		private const Evidences FreezingTemperatures = Evidences.FreezingTemperatures;
		private const Evidences GhostOrb = Evidences.GhostOrb;
		private const Evidences GhostWriting = Evidences.GhostWriting;
		private const Evidences SpiritBox = Evidences.SpiritBox;

		private const Ghost Banshee = Ghost.Banshee;
		private const Ghost Demon = Ghost.Demon;
		private const Ghost Jinn = Ghost.Jinn;
		private const Ghost Mare = Ghost.Mare;
		private const Ghost Oni = Ghost.Oni;
		private const Ghost Phantom = Ghost.Phantom;
		private const Ghost Poltergeist = Ghost.Poltergeist;
		private const Ghost Revenant = Ghost.Revenant;
		private const Ghost Shade = Ghost.Shade;
		private const Ghost Spirit = Ghost.Spirit;
		private const Ghost Wraith = Ghost.Wraith;
		private const Ghost Yurei = Ghost.Yurei;

		[Theory]
		[InlineData(Banshee)]
		[InlineData(Demon)]
		[InlineData(Jinn)]
		[InlineData(Mare)]
		[InlineData(Oni)]
		[InlineData(Phantom)]
		[InlineData(Poltergeist)]
		[InlineData(Revenant)]
		[InlineData(Shade)]
		[InlineData(Spirit)]
		[InlineData(Wraith)]
		[InlineData(Yurei)]

		public void GhostsHaveThreeEvidences(Ghost ghost)
		{
			var evidences = ghost.GetEvidences().ToList();

			Assert.Equal(3, evidences.Count);
			Assert.Equal(3, evidences.Distinct().Count());
			Assert.Equal(3, evidences.Where(e => (int)e > 0).Count());
		}

		[Theory]
		// one piece of evidence
		[InlineData(EmfLevelFive, Banshee, Jinn, Oni, Phantom, Revenant, Shade)]
		[InlineData(Fingerprints, Banshee, Poltergeist, Revenant, Spirit, Wraith)]
		[InlineData(FreezingTemperatures, Banshee, Demon, Mare, Phantom, Wraith, Yurei)]
		[InlineData(GhostOrb, Jinn, Mare, Phantom, Poltergeist, Shade, Yurei)]
		[InlineData(GhostWriting, Demon, Oni, Revenant, Shade, Spirit, Yurei)]
		[InlineData(SpiritBox, Demon, Jinn, Mare, Oni, Poltergeist, Spirit, Wraith)]

		// two pieces
		[InlineData(EmfLevelFive | Fingerprints, Banshee, Revenant)]
		[InlineData(EmfLevelFive | FreezingTemperatures, Banshee, Phantom)]
		[InlineData(EmfLevelFive | GhostOrb, Jinn, Phantom, Shade)]
		[InlineData(EmfLevelFive | GhostWriting, Oni, Revenant, Shade)]
		[InlineData(EmfLevelFive | SpiritBox, Jinn, Oni)]

		[InlineData(Fingerprints | FreezingTemperatures, Banshee, Wraith)]
		[InlineData(Fingerprints | GhostOrb, Poltergeist)]
		[InlineData(Fingerprints | GhostWriting, Revenant, Spirit)]
		[InlineData(Fingerprints | SpiritBox, Poltergeist, Spirit, Wraith)]

		[InlineData(FreezingTemperatures | GhostOrb, Mare, Phantom, Yurei)]
		[InlineData(FreezingTemperatures | GhostWriting, Demon, Yurei)]
		[InlineData(FreezingTemperatures | SpiritBox, Demon, Mare, Wraith)]

		[InlineData(GhostOrb | GhostWriting, Shade, Yurei)]
		[InlineData(GhostOrb | SpiritBox, Jinn, Mare, Poltergeist)]

		[InlineData(GhostWriting | SpiritBox, Demon, Oni, Spirit)]

		// three
		[InlineData(EmfLevelFive | Fingerprints | FreezingTemperatures, Banshee)]
		[InlineData(EmfLevelFive | Fingerprints | GhostOrb)]
		[InlineData(EmfLevelFive | Fingerprints | GhostWriting, Revenant)]
		[InlineData(EmfLevelFive | Fingerprints | SpiritBox)]

		[InlineData(EmfLevelFive | FreezingTemperatures | GhostOrb, Phantom)]
		[InlineData(EmfLevelFive | FreezingTemperatures | GhostWriting)]
		[InlineData(EmfLevelFive | FreezingTemperatures | SpiritBox)]

		[InlineData(EmfLevelFive | GhostOrb | GhostWriting, Shade)]
		[InlineData(EmfLevelFive | GhostOrb | SpiritBox, Jinn)]

		[InlineData(EmfLevelFive | GhostWriting | SpiritBox, Oni)]

		[InlineData(Fingerprints | FreezingTemperatures | GhostOrb)]
		[InlineData(Fingerprints | FreezingTemperatures | GhostWriting)]
		[InlineData(Fingerprints | FreezingTemperatures | SpiritBox, Wraith)]

		[InlineData(Fingerprints | GhostOrb | GhostWriting)]
		[InlineData(Fingerprints | GhostOrb | SpiritBox, Poltergeist)]

		[InlineData(Fingerprints | GhostWriting | SpiritBox, Spirit)]

		[InlineData(FreezingTemperatures | GhostOrb | GhostWriting, Yurei)]
		[InlineData(FreezingTemperatures | GhostOrb | SpiritBox, Mare)]

		[InlineData(FreezingTemperatures | GhostWriting | SpiritBox, Demon)]

		[InlineData(GhostOrb | GhostWriting | SpiritBox)]

		public void PossibleGhostsFromEvidence(Evidences evidences, params Ghost[] expected)
		{
			var actual = evidences.GetGhosts().ToList();

			Assert.Equal(expected.Length, actual.Count);
			Assert.Equal(expected.OrderBy(o => o), actual.OrderBy(o => o));
		}

		[Theory]
		// one piece of evidence
		[InlineData(EmfLevelFive, Demon, Mare, Poltergeist, Spirit, Wraith, Yurei)]
		[InlineData(Fingerprints, Demon, Jinn, Mare, Oni, Phantom, Shade, Yurei)]
		[InlineData(FreezingTemperatures, Jinn, Oni, Poltergeist, Revenant, Shade, Spirit)]
		[InlineData(GhostOrb, Banshee, Demon, Oni, Revenant, Spirit, Wraith)]
		[InlineData(GhostWriting, Banshee, Jinn, Mare, Phantom, Poltergeist, Wraith)]
		[InlineData(SpiritBox, Banshee, Phantom, Revenant, Shade, Yurei)]

		// two pieces
		[InlineData(EmfLevelFive | Fingerprints, Demon, Mare, Yurei)]
		[InlineData(EmfLevelFive | FreezingTemperatures, Poltergeist, Spirit)]
		[InlineData(EmfLevelFive | GhostOrb, Demon, Spirit, Wraith)]
		[InlineData(EmfLevelFive | GhostWriting, Mare, Poltergeist, Wraith)]
		[InlineData(EmfLevelFive | SpiritBox, Yurei)]

		[InlineData(Fingerprints | FreezingTemperatures, Jinn, Oni, Shade)]
		[InlineData(Fingerprints | GhostOrb, Demon, Oni)]
		[InlineData(Fingerprints | GhostWriting, Jinn, Mare, Phantom)]
		[InlineData(Fingerprints | SpiritBox, Phantom, Shade, Yurei)]

		[InlineData(FreezingTemperatures | GhostOrb, Oni, Revenant, Spirit)]
		[InlineData(FreezingTemperatures | GhostWriting, Jinn, Poltergeist)]
		[InlineData(FreezingTemperatures | SpiritBox, Revenant, Shade)]

		[InlineData(GhostOrb | GhostWriting, Banshee, Wraith)]
		[InlineData(GhostOrb | SpiritBox, Banshee, Revenant)]

		[InlineData(GhostWriting | SpiritBox, Banshee, Phantom)]

		// three
		[InlineData(EmfLevelFive | Fingerprints | FreezingTemperatures)]
		[InlineData(EmfLevelFive | Fingerprints | GhostOrb, Demon)]
		[InlineData(EmfLevelFive | Fingerprints | GhostWriting, Mare)]
		[InlineData(EmfLevelFive | Fingerprints | SpiritBox, Yurei)]

		[InlineData(EmfLevelFive | FreezingTemperatures | GhostOrb, Spirit)]
		[InlineData(EmfLevelFive | FreezingTemperatures | GhostWriting, Poltergeist)]
		[InlineData(EmfLevelFive | FreezingTemperatures | SpiritBox)]

		[InlineData(EmfLevelFive | GhostOrb | GhostWriting, Wraith)]
		[InlineData(EmfLevelFive | GhostOrb | SpiritBox)]

		[InlineData(EmfLevelFive | GhostWriting | SpiritBox)]

		[InlineData(Fingerprints | FreezingTemperatures | GhostOrb, Oni)]
		[InlineData(Fingerprints | FreezingTemperatures | GhostWriting, Jinn)]
		[InlineData(Fingerprints | FreezingTemperatures | SpiritBox, Shade)]

		[InlineData(Fingerprints | GhostOrb | GhostWriting)]
		[InlineData(Fingerprints | GhostOrb | SpiritBox)]

		[InlineData(Fingerprints | GhostWriting | SpiritBox, Phantom)]

		[InlineData(FreezingTemperatures | GhostOrb | GhostWriting)]
		[InlineData(FreezingTemperatures | GhostOrb | SpiritBox, Revenant)]

		[InlineData(FreezingTemperatures | GhostWriting | SpiritBox)]

		[InlineData(GhostOrb | GhostWriting | SpiritBox, Banshee)]
		public void GhostsFromEliminatedEvidence(Evidences eliminated, params Ghost[] expected)
		{
			var actual = eliminated.GetEliminatedGhosts().ToList();

			Assert.Equal(expected.Length, actual.Count);
			Assert.Equal(expected.OrderBy(o => o), actual.OrderBy(o => o));
		}

		[Theory]
		// one of each
		[InlineData(EmfLevelFive, Fingerprints, Jinn, Oni, Phantom, Shade)]
		[InlineData(EmfLevelFive, FreezingTemperatures, Jinn, Oni, Revenant, Shade)]
		[InlineData(EmfLevelFive, GhostOrb, Banshee, Oni, Revenant)]
		[InlineData(EmfLevelFive, GhostWriting, Banshee, Jinn, Phantom)]
		[InlineData(EmfLevelFive, SpiritBox, Banshee, Phantom, Revenant, Shade)]

		[InlineData(Fingerprints, EmfLevelFive, Poltergeist, Spirit, Wraith)]
		[InlineData(Fingerprints, FreezingTemperatures, Poltergeist, Revenant, Spirit)]
		[InlineData(Fingerprints, GhostOrb, Banshee, Revenant, Spirit, Wraith)]
		[InlineData(Fingerprints, GhostWriting, Banshee, Poltergeist, Wraith)]
		[InlineData(Fingerprints, SpiritBox, Banshee, Revenant)]

		[InlineData(FreezingTemperatures, EmfLevelFive, Demon, Mare, Wraith, Yurei)]
		[InlineData(FreezingTemperatures, Fingerprints, Demon, Mare, Phantom, Yurei)]
		[InlineData(FreezingTemperatures, GhostOrb, Banshee, Demon, Wraith)]
		[InlineData(FreezingTemperatures, GhostWriting, Banshee, Mare, Phantom, Wraith)]
		[InlineData(FreezingTemperatures, SpiritBox, Banshee, Phantom, Yurei)]

		[InlineData(GhostOrb, EmfLevelFive, Mare, Poltergeist, Yurei)]
		[InlineData(GhostOrb, Fingerprints, Jinn, Mare, Phantom, Shade, Yurei)]
		[InlineData(GhostOrb, FreezingTemperatures, Jinn, Poltergeist, Shade)]
		[InlineData(GhostOrb, GhostWriting, Jinn, Mare, Phantom, Poltergeist)]
		[InlineData(GhostOrb, SpiritBox, Phantom, Shade, Yurei)]

		[InlineData(GhostWriting, EmfLevelFive, Demon, Spirit, Yurei)]
		[InlineData(GhostWriting, Fingerprints, Demon, Oni, Shade, Yurei)]
		[InlineData(GhostWriting, FreezingTemperatures, Oni, Revenant, Shade, Spirit)]
		[InlineData(GhostWriting, GhostOrb, Demon, Oni, Revenant, Spirit)]
		[InlineData(GhostWriting, SpiritBox, Revenant, Shade, Yurei)]

		[InlineData(SpiritBox, EmfLevelFive, Demon, Mare, Poltergeist, Spirit, Wraith)]
		[InlineData(SpiritBox, Fingerprints, Demon, Jinn, Mare, Oni)]
		[InlineData(SpiritBox, FreezingTemperatures, Jinn, Oni, Poltergeist, Spirit)]
		[InlineData(SpiritBox, GhostOrb, Demon, Oni, Spirit, Wraith)]
		[InlineData(SpiritBox, GhostWriting, Jinn, Mare, Poltergeist, Wraith)]

		// two found, one eliminated
		[InlineData(EmfLevelFive | Fingerprints, FreezingTemperatures, Revenant)]
		[InlineData(EmfLevelFive | Fingerprints, GhostOrb, Banshee, Revenant)]
		[InlineData(EmfLevelFive | Fingerprints, GhostWriting, Banshee)]
		[InlineData(EmfLevelFive | Fingerprints, SpiritBox, Banshee, Revenant)]

		[InlineData(EmfLevelFive | FreezingTemperatures, Fingerprints, Phantom)]
		[InlineData(EmfLevelFive | FreezingTemperatures, GhostOrb, Banshee)]
		[InlineData(EmfLevelFive | FreezingTemperatures, GhostWriting, Banshee, Phantom)]
		[InlineData(EmfLevelFive | FreezingTemperatures, SpiritBox, Banshee, Phantom)]

		[InlineData(EmfLevelFive | GhostOrb, Fingerprints, Jinn, Phantom, Shade)]
		[InlineData(EmfLevelFive | GhostOrb, FreezingTemperatures, Jinn, Shade)]
		[InlineData(EmfLevelFive | GhostOrb, GhostWriting, Jinn, Phantom)]
		[InlineData(EmfLevelFive | GhostOrb, SpiritBox, Phantom, Shade)]

		[InlineData(EmfLevelFive | GhostWriting, Fingerprints, Oni, Shade)]
		[InlineData(EmfLevelFive | GhostWriting, FreezingTemperatures, Oni, Revenant, Shade)]
		[InlineData(EmfLevelFive | GhostWriting, GhostOrb, Oni, Revenant)]
		[InlineData(EmfLevelFive | GhostWriting, SpiritBox, Revenant, Shade)]

		[InlineData(EmfLevelFive | SpiritBox, Fingerprints, Jinn, Oni)]
		[InlineData(EmfLevelFive | SpiritBox, FreezingTemperatures, Jinn, Oni)]
		[InlineData(EmfLevelFive | SpiritBox, GhostOrb, Oni)]
		[InlineData(EmfLevelFive | SpiritBox, GhostWriting, Jinn)]

		[InlineData(Fingerprints | FreezingTemperatures, EmfLevelFive, Wraith)]
		[InlineData(Fingerprints | FreezingTemperatures, GhostOrb, Banshee, Wraith)]
		[InlineData(Fingerprints | FreezingTemperatures, GhostWriting, Banshee, Wraith)]
		[InlineData(Fingerprints | FreezingTemperatures, SpiritBox, Banshee)]

		[InlineData(Fingerprints | GhostOrb, EmfLevelFive, Poltergeist)]
		[InlineData(Fingerprints | GhostOrb, FreezingTemperatures, Poltergeist)]
		[InlineData(Fingerprints | GhostOrb, GhostWriting, Poltergeist)]
		[InlineData(Fingerprints | GhostOrb, SpiritBox)]

		[InlineData(Fingerprints | GhostWriting, EmfLevelFive, Spirit)]
		[InlineData(Fingerprints | GhostWriting, FreezingTemperatures, Revenant, Spirit)]
		[InlineData(Fingerprints | GhostWriting, GhostOrb, Revenant, Spirit)]
		[InlineData(Fingerprints | GhostWriting, SpiritBox, Revenant)]

		[InlineData(Fingerprints | SpiritBox, EmfLevelFive, Poltergeist, Spirit, Wraith)]
		[InlineData(Fingerprints | SpiritBox, FreezingTemperatures, Poltergeist, Spirit)]
		[InlineData(Fingerprints | SpiritBox, GhostOrb, Spirit, Wraith)]
		[InlineData(Fingerprints | SpiritBox, GhostWriting, Poltergeist, Wraith)]

		[InlineData(FreezingTemperatures | GhostOrb, EmfLevelFive, Mare, Yurei)]
		[InlineData(FreezingTemperatures | GhostOrb, Fingerprints, Mare, Phantom, Yurei)]
		[InlineData(FreezingTemperatures | GhostOrb, GhostWriting, Mare, Phantom)]
		[InlineData(FreezingTemperatures | GhostOrb, SpiritBox, Phantom, Yurei)]

		[InlineData(FreezingTemperatures | GhostWriting, EmfLevelFive, Demon, Yurei)]
		[InlineData(FreezingTemperatures | GhostWriting, Fingerprints, Demon, Yurei)]
		[InlineData(FreezingTemperatures | GhostWriting, GhostOrb, Demon)]
		[InlineData(FreezingTemperatures | GhostWriting, SpiritBox, Yurei)]

		[InlineData(FreezingTemperatures | SpiritBox, EmfLevelFive, Demon, Mare, Wraith)]
		[InlineData(FreezingTemperatures | SpiritBox, Fingerprints, Demon, Mare)]
		[InlineData(FreezingTemperatures | SpiritBox, GhostOrb, Demon, Wraith)]
		[InlineData(FreezingTemperatures | SpiritBox, GhostWriting, Mare, Wraith)]

		[InlineData(GhostOrb | GhostWriting, EmfLevelFive, Yurei)]
		[InlineData(GhostOrb | GhostWriting, Fingerprints, Shade, Yurei)]
		[InlineData(GhostOrb | GhostWriting, FreezingTemperatures, Shade)]
		[InlineData(GhostOrb | GhostWriting, SpiritBox, Shade, Yurei)]

		[InlineData(GhostOrb | SpiritBox, EmfLevelFive, Mare, Poltergeist)]
		[InlineData(GhostOrb | SpiritBox, Fingerprints, Jinn, Mare)]
		[InlineData(GhostOrb | SpiritBox, FreezingTemperatures, Jinn, Poltergeist)]
		[InlineData(GhostOrb | SpiritBox, GhostWriting, Jinn, Mare, Poltergeist)]

		// one found, two eliminated
		[InlineData(EmfLevelFive, Fingerprints | FreezingTemperatures, Jinn, Oni, Shade)]
		[InlineData(EmfLevelFive, Fingerprints | GhostOrb, Oni)]
		[InlineData(EmfLevelFive, Fingerprints | GhostWriting, Jinn, Phantom)]
		[InlineData(EmfLevelFive, Fingerprints | SpiritBox, Phantom, Shade)]
		[InlineData(EmfLevelFive, FreezingTemperatures | GhostOrb, Oni, Revenant)]
		[InlineData(EmfLevelFive, FreezingTemperatures | GhostWriting, Jinn)]
		[InlineData(EmfLevelFive, FreezingTemperatures | SpiritBox, Revenant, Shade)]
		[InlineData(EmfLevelFive, GhostOrb | GhostWriting, Banshee)]
		[InlineData(EmfLevelFive, GhostOrb | SpiritBox, Banshee, Revenant)]
		[InlineData(EmfLevelFive, GhostWriting | SpiritBox, Banshee, Phantom)]

		[InlineData(Fingerprints, EmfLevelFive | FreezingTemperatures, Poltergeist, Spirit)]
		[InlineData(Fingerprints, EmfLevelFive | GhostOrb, Spirit, Wraith)]
		[InlineData(Fingerprints, EmfLevelFive | GhostWriting, Poltergeist, Wraith)]
		[InlineData(Fingerprints, EmfLevelFive | SpiritBox)]
		[InlineData(Fingerprints, FreezingTemperatures | GhostOrb, Revenant, Spirit)]
		[InlineData(Fingerprints, FreezingTemperatures | GhostWriting, Poltergeist)]
		[InlineData(Fingerprints, FreezingTemperatures | SpiritBox, Revenant)]
		[InlineData(Fingerprints, GhostOrb | GhostWriting, Banshee, Wraith)]
		[InlineData(Fingerprints, GhostOrb | SpiritBox, Banshee, Revenant)]
		[InlineData(Fingerprints, GhostWriting | SpiritBox, Banshee)]

		[InlineData(FreezingTemperatures, EmfLevelFive | Fingerprints, Demon, Mare, Yurei)]
		[InlineData(FreezingTemperatures, EmfLevelFive | GhostOrb, Demon, Wraith)]
		[InlineData(FreezingTemperatures, EmfLevelFive | GhostWriting, Mare, Wraith)]
		[InlineData(FreezingTemperatures, EmfLevelFive | SpiritBox, Yurei)]
		[InlineData(FreezingTemperatures, Fingerprints | GhostOrb, Demon)]
		[InlineData(FreezingTemperatures, Fingerprints | GhostWriting, Mare, Phantom)]
		[InlineData(FreezingTemperatures, Fingerprints | SpiritBox, Phantom, Yurei)]
		[InlineData(FreezingTemperatures, GhostOrb | GhostWriting, Banshee, Wraith)]
		[InlineData(FreezingTemperatures, GhostOrb | SpiritBox, Banshee)]
		[InlineData(FreezingTemperatures, GhostWriting | SpiritBox, Banshee, Phantom)]

		[InlineData(GhostOrb, EmfLevelFive | Fingerprints, Mare, Yurei)]
		[InlineData(GhostOrb, EmfLevelFive | FreezingTemperatures, Poltergeist)]
		[InlineData(GhostOrb, EmfLevelFive | GhostWriting, Mare, Poltergeist)]
		[InlineData(GhostOrb, EmfLevelFive | SpiritBox, Yurei)]
		[InlineData(GhostOrb, Fingerprints | FreezingTemperatures, Jinn, Shade)]
		[InlineData(GhostOrb, Fingerprints | GhostWriting, Jinn, Mare, Phantom)]
		[InlineData(GhostOrb, Fingerprints | SpiritBox, Phantom, Shade, Yurei)]
		[InlineData(GhostOrb, FreezingTemperatures | GhostWriting, Jinn, Poltergeist)]
		[InlineData(GhostOrb, FreezingTemperatures | SpiritBox, Shade)]
		[InlineData(GhostOrb, GhostWriting | SpiritBox, Phantom)]
		public void GhostsFromFoundAndEliminatedEvidence(Evidences found, Evidences eliminated, params Ghost[] expected)
		{
			var actual = found.GetGhosts(eliminated).ToList();

			Assert.Equal(expected.Length, actual.Count);
			Assert.Equal(expected.OrderBy(o => o), actual.OrderBy(o => o));
		}
	}
}
