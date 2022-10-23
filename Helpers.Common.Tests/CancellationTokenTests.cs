using System.Diagnostics;
using Xunit;

namespace Helpers.Common.Tests;

public class CancellationTokenTests
{
	[Theory]
	[InlineData(500)]
	[InlineData(1_000)]
	[InlineData(2_000)]
	public async Task WithCancellationToken(int timeout)
	{
		// Arrange
		using var cts = new CancellationTokenSource(millisecondsDelay: timeout);
		var task = Async(cts.Token);

		// Act
		var stopwatch = Stopwatch.StartNew();
		try { await task; } catch (TaskCanceledException) { }
		stopwatch.Stop();

		// Assert
		Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, timeout * .9, timeout * 1.1);
	}

	[Theory]
	[InlineData(500)]
	[InlineData(1_000)]
	[InlineData(2_000)]
	public async Task WithoutCancellationToken(int timeout)
	{
		// Arrange
		var task = Task.WhenAny(
			Async(),
			Task.Delay(timeout));

		// Act
		var stopwatch = Stopwatch.StartNew();
		await task;
		stopwatch.Stop();

		// Assert
		Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, timeout * .9, timeout * 1.1);
	}

	[Theory]
	[InlineData(1_000, "https://httpbin.org/get")]
	[InlineData(100, "https://httpbin.org/get")]
	public async Task HttpBinTests_WithCancellationToken(int timeout, string requestUri)
	{
		// Arrange
		using var cts = new CancellationTokenSource(millisecondsDelay: timeout);
		using var httpClient = new HttpClient();
		var task = GetStringAsync(httpClient, new Uri(requestUri), cts.Token);

		// Act
		var stopwatch = Stopwatch.StartNew();
		try { await task; } catch (TaskCanceledException) { }
		stopwatch.Stop();

		// Assert
		Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 0, timeout * 1.2);
	}

	[Theory]
	[InlineData(1_000, "https://httpbin.org/get")]
	[InlineData(100, "https://httpbin.org/get")]
	public async Task HttpBinTests_WithoutCancellationToken(int timeout, string requestUri)
	{
		// Arrange
		using var httpClient = new HttpClient();
		var task = Task.WhenAny(
			GetStringAsync(httpClient, new Uri(requestUri)),
			Task.Delay(timeout));

		// Act
		var stopwatch = Stopwatch.StartNew();
		try { await task; } catch (TaskCanceledException) { }
		stopwatch.Stop();

		// Assert
		Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 0, timeout * 1.2);
	}

	private static async Task Async(CancellationToken cancellationToken = default)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			await Task.Delay(millisecondsDelay: 100, cancellationToken);
		}
	}

	private static Task<string> GetStringAsync(HttpClient httpClient, Uri requestUri, CancellationToken cancellationToken = default)
		=> httpClient.GetStringAsync(requestUri, cancellationToken);
}
