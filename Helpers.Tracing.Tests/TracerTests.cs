using Moq;
using OpenTracing;
using System.IO;
using Xunit;

namespace Helpers.Tracing.Tests
{
	public class TracerTests
	{
		[Theory]
		[InlineData("test", default, "test")]
		[InlineData("test=>test", "test", "test")]
		[InlineData(
			"ExtensionMethodsTests=>ExtensionMethodsTests_BuildDefaultSpan",
			"ExtensionMethodsTests_BuildDefaultSpan",
			"c", "code", "Helpers", "Helpers.Tracing.Tests", "ExtensionMethodsTests.cs")]
		public void BuildDefaultSpan(
			string expected,
			string methodName,
			params string[] paths)
		{
			// Arrange
			var filePath = Path.Combine(paths);
			var tracerMock = new Mock<ITracer>();

			tracerMock
				.Setup(t => t.BuildSpan(It.IsAny<string>()))
				.Returns((string s) => new MockSpanBuilder(s));

			// Act
			var span = tracerMock.Object.BuildDefaultSpan(methodName, filePath);

			// Assert
			Assert.Equal(expected, (span as MockSpanBuilder)?.OperationName);
		}
	}
}
