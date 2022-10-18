namespace Helpers.Timing;

[Flags]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "synonyms")]
public enum Units : byte
{
	None = 0,
	Day = 1,
	Days = 1,
	Hour = 2,
	Hours = 2,
	Millisecond = 4,
	Milliseconds = 4,
	Minute = 8,
	Minutes = 8,
	Second = 16,
	Seconds = 16,
}
