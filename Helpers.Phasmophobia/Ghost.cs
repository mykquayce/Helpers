namespace Helpers.Phasmophobia
{
	// https://phasmophobia.fandom.com/wiki/Evidence#Ghost_Evidence
	public enum Ghost : byte
	{
		Banshee = Evidences.EmfLevelFive | Evidences.Fingerprints | Evidences.FreezingTemperatures,
		Demon = Evidences.FreezingTemperatures | Evidences.GhostWriting | Evidences.SpiritBox,
		Jinn = Evidences.EmfLevelFive | Evidences.GhostOrb | Evidences.SpiritBox,
		Mare = Evidences.FreezingTemperatures | Evidences.GhostOrb | Evidences.SpiritBox,
		Oni = Evidences.EmfLevelFive | Evidences.GhostWriting | Evidences.SpiritBox,
		Phantom = Evidences.EmfLevelFive | Evidences.FreezingTemperatures | Evidences.GhostOrb,
		Poltergeist = Evidences.Fingerprints | Evidences.GhostOrb | Evidences.SpiritBox,
		Revenant = Evidences.EmfLevelFive | Evidences.Fingerprints | Evidences.GhostWriting,
		Shade = Evidences.EmfLevelFive | Evidences.GhostOrb | Evidences.GhostWriting,
		Spirit = Evidences.Fingerprints | Evidences.GhostWriting | Evidences.SpiritBox,
		Wraith = Evidences.Fingerprints | Evidences.FreezingTemperatures | Evidences.SpiritBox,
		Yurei = Evidences.FreezingTemperatures | Evidences.GhostOrb | Evidences.GhostWriting,
	}
}
