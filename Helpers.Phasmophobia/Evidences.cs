using System;

namespace Helpers.Phasmophobia
{
	[Flags]
	public enum Evidences : byte
	{
		EmfLevelFive = 1,
		Fingerprints = 2,
		FreezingTemperatures = 4,
		GhostOrb = 8,
		GhostWriting = 16,
		SpiritBox = 32,
	}
}
