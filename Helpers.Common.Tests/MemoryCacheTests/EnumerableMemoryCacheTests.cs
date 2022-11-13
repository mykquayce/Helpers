using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Helpers.Common.Tests.MemoryCacheTests;

public class EnumerableMemoryCacheTests
{
	[Theory]
	[InlineData("key")]
	[InlineData("key1", "key2")]
	public void EnumerableMemoryCache_Add_Remove(params string[] keys)
	{
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());

		Assert.Empty(sut);

		foreach (var key in keys)
		{
			sut.Set(key, "value");
		}

		Assert.NotEmpty(sut);

		foreach (var key in keys)
		{
			sut.Remove(key);
		}

		Assert.Empty(sut);
	}

	[Theory]
	[InlineData("key")]
	[InlineData("key1", "key2")]
	public void EnumerableMemoryCache_AddIndexed_Remove(params string[] keys)
	{
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());

		Assert.Empty(sut);

		foreach (var key in keys)
		{
			sut[key] = "value";
		}

		Assert.NotEmpty(sut);

		foreach (var key in keys)
		{
			sut.Remove(key);
		}

		Assert.Empty(sut);
	}

	[Theory]
	[InlineData("key")]
	[InlineData("key1", "key2")]
	public void EnumerableMemoryCache_Keys(params object[] keys)
	{
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());

		Assert.Empty(sut);

		foreach (var key in keys)
		{
			sut[key] = "value";
		}

		var actual = sut.Keys;
		Assert.NotEmpty(actual);
		Assert.Equal(keys.Length, actual.Count);
		Assert.All(keys, key => Assert.Contains(key, sut.Keys));
		Assert.All(sut.Keys, key => Assert.Contains(key, keys));
	}

	[Theory]
	[InlineData("key")]
	[InlineData("key1", "key2")]
	public void EnumerableMemoryCache_Values(params string[] keys)
	{
		var expected = Enumerable.Repeat("value", count: keys.Length);
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());

		Assert.Empty(sut);

		foreach (var key in keys)
		{
			sut[key] = "value";
		}

		Assert.NotEmpty(sut);
		Assert.Equal(expected, sut.Values);
	}

	[Theory]
	[InlineData("key", "value")]
	public void EnumerableMemoryCache_Contains(object key, object? value)
	{
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());
		sut[key] = value;
		var actual = sut.Contains(new KeyValuePair<object, object?>(key, value));
		Assert.True(actual);
	}

	[Theory]
	[InlineData("key", "value")]
	public void EnumerableMemoryCache_ContainsKey(object key, object? value)
	{
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());
		sut[key] = value;
		var actual = sut.ContainsKey(key);
		Assert.True(actual);
	}

	[Theory]
	[InlineData("key")]
	[InlineData("key1", "key2")]
	public void MemoryCache_Add_Expire(params string[] keys)
	{
		using var sut = new MemoryCache(new MemoryCacheOptions());
		foreach (var key in keys) { sut.Set(key, "value", DateTime.UtcNow.AddMilliseconds(100)); }
		foreach (var key in keys) { Assert.True(sut.TryGetValue(key, out _)); }
		Thread.Sleep(millisecondsTimeout: 200);
		foreach (var key in keys) { Assert.False(sut.TryGetValue(key, out _)); }
	}

	[Theory]
	[InlineData("key")]
	[InlineData("key1", "key2")]
	public void EnumerableMemoryCache_Add_Expire(params string[] keys)
	{
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());
		Assert.Empty(sut);
		foreach (var key in keys) { sut.Set(key, "value", DateTime.UtcNow.AddMilliseconds(100)); }
		foreach (var key in keys) { Assert.True(sut.TryGetValue(key, out _)); }
		Assert.NotEmpty(sut);
		Thread.Sleep(millisecondsTimeout: 200);
		foreach (var key in keys) { Assert.False(sut.TryGetValue(key, out _)); }
		Assert.Empty(sut);
	}

	[Fact]
	public void MemoryCache_AddTwo_AddOne_ExpireOne()
	{
		// Arrange
		using var sut = new MemoryCache(new MemoryCacheOptions());

		// Assert empty
		Assert.Equal(0, sut.Count);

		// Arrange: add
		sut.Set("key1", "value");
		sut.Set("key2", "value");
		sut.Set("key3", "value", DateTime.UtcNow.AddMilliseconds(100));

		// Assert added
		Assert.True(tryget("key1"));
		Assert.True(tryget("key2"));
		Assert.True(tryget("key3"));

		// Arrange: wait
		Thread.Sleep(millisecondsTimeout: 200);

		// Assert removed
		Assert.False(tryget("key3"));

		bool tryget(string key) => sut.TryGetValue(key, out _);
	}

	[Fact]
	public void EnumerableMemoryCache_AddTwo_AddOne_ExpireOne()
	{
		// Arrange
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());

		// Assert empty
		Assert.Empty(sut);

		// Arrange: add
		sut.Set("key1", "value");
		sut.Set("key2", "value");
		sut.Set("key3", "value", DateTime.UtcNow.AddMilliseconds(100));

		// Assert added
		Assert.Equal(3, sut.Count);

		// Arrange: wait
		Thread.Sleep(millisecondsTimeout: 200);

		// Assert removed
		Assert.Equal(2, sut.Count);
	}

	[Theory]
	[InlineData("key", "value1")]
	[InlineData("key", "value1", "value2")]
	[InlineData("key", "value1", "value2", "value3")]
	public void EnumerableMemoryCache_AddManyWithSameKey(string key, params string[] values)
	{
		// Arrange
		using var sut = new EnumerableMemoryCache(new MemoryCacheOptions());

		// Act
		foreach (var value in values) { sut.Set(key, value); }

		// Assert: matches last
		Assert.Single(sut);
		Assert.Equal(values.Last(), sut.Get(key));
	}
}
