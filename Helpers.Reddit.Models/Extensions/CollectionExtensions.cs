namespace System.Collections.Generic;

public static class CollectionExtensions
{
	public static void Deconstruct<T>(this IEnumerable<T> value, out T first, out T second)
		=> value.Deconstruct(out first!, out second!, out _, out _, out _, out _);

	public static void Deconstruct<T>(this IEnumerable<T> value, out T first, out T second, out T third)
		=> value.Deconstruct(out first!, out second!, out third!, out _, out _, out _);

	public static void Deconstruct<T>(this IEnumerable<T> value, out T first, out T second, out T third, out T fourth)
		=> value.Deconstruct(out first!, out second!, out third!, out fourth!, out _, out _);

	public static void Deconstruct<T>(this IEnumerable<T> value, out T first, out T second, out T third, out T fourth, out T fifth)
		=> value.Deconstruct(out first!, out second!, out third!, out fourth!, out fifth!, out _);

	public static void Deconstruct<T>(this IEnumerable<T> value, out T? first, out T? second, out T? third, out T? fourth, out T? fifth, out T? sixth)
	{
		using var enumerator = value.GetEnumerator();
		first = f();
		second = f();
		third = f();
		fourth = f();
		fifth = f();
		sixth = f();

		T? f() => enumerator.MoveNext() ? enumerator.Current : default;
	}
}
