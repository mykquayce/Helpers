using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Helpers.DelegatingHandlers.Tests;

public class LoggingHandlerTests
{
	private static readonly ICollection<string> _lines = [];

	[Theory]
	[InlineData(406, "NotAcceptable")]
	[InlineData(502, "BadGateway")]
	public async Task Test1(short code, string expected)
	{
		using var provider = new ServiceCollection()
			.AddLogging(b =>
			{
				ILoggerProvider provider = new LoggerProvider();
				b.AddProvider(provider);
			})
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddTransient<LoggingHandler>()
			.AddTransient<MockingHandler>()
			.AddHttpClient<TestClient>(c => c.BaseAddress = new Uri("http://localhost/"))
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<LoggingHandler>()
				.AddHttpMessageHandler<MockingHandler>()
				.Services
			.BuildServiceProvider();

		var client = provider.GetRequiredService<TestClient>();

		// Act
		var actual = await client.GetStatusCodeAsync(code);

		// Assert
		Assert.Equal(expected, actual);
		Assert.NotEmpty(_lines);
		Assert.Contains(_lines, s => s.StartsWith("Erro System.Net.Http.LoggingHandler", StringComparison.OrdinalIgnoreCase));
	}

	private class LoggerProvider : ILoggerProvider
	{
		public ILogger CreateLogger(string categoryName) => new Logger(categoryName);
		public void Dispose() { }
	}

	private class Logger(string categoryName) : ILogger
	{
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
		public bool IsEnabled(LogLevel logLevel) => true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			string line = logLevel.ToString("G")[..4] + " " + categoryName + Environment.NewLine + "    " + formatter(state, exception);
			_lines.Add(line);
		}
	}
}
