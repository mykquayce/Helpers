namespace System.Text.RegularExpressions;

public static class RegexExtensions
{
	public static void Deconstruct(this Match match, out string? first, out string? second)
		=> Deconstruct(match, out first, out second, out _, out _, out _, out _, out _);
	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third)
		=> Deconstruct(match, out first, out second, out third, out _, out _, out _, out _);
	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth)
		=> Deconstruct(match, out first, out second, out third, out fourth, out _, out _, out _);
	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth, out string? fifth)
		=> Deconstruct(match, out first, out second, out third, out fourth, out fifth, out _, out _);
	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth, out string? fifth, out string? sixth)
		=> Deconstruct(match, out first, out second, out third, out fourth, out fifth, out sixth, out _);

	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth, out string? fifth, out string? sixth, out string? seventh)
	{
		var enumerator = match.Groups.GetEnumerator();

		string? f() => enumerator!.MoveNext() ? ((Group)enumerator.Current).Value : default;

		first = f();
		second = f();
		third = f();
		fourth = f();
		fifth = f();
		sixth = f();
		seventh = f();
	}
}
