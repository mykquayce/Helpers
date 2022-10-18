namespace System.Collections.Generic;

public static class CollectionExtensions
{
	public static void Deconstruct<T>(this IEnumerable<T> items, out T first, out T second) => (first, second, _, _, _, _) = items;
	public static void Deconstruct<T>(this IEnumerable<T> items, out T first, out T second, out T third) => (first, second, third, _, _, _) = items;
	public static void Deconstruct<T>(this IEnumerable<T> items, out T first, out T second, out T third, out T fourth) => (first, second, third, fourth, _, _) = items;
	public static void Deconstruct<T>(this IEnumerable<T> items, out T first, out T second, out T third, out T fourth, out T fifth) => (first, second, third, fourth, fifth, _) = items;

	public static void Deconstruct<T>(this IEnumerable<T> items, out T first, out T second, out T third, out T fourth, out T fifth, out T sixth)
	{
		using var enumerator = items.GetEnumerator();

		first = f();
		second = f();
		third = f();
		fourth = f();
		fifth = f();
		sixth = f();

		T f() => enumerator.MoveNext() ? enumerator.Current : default!;
	}
}
