using Moq;
using OpenTracing;
using OpenTracing.Tag;
using System;
using Xunit;

namespace Helpers.Tracing.Tests
{
	public class ExtensionMethodsTests
	{
		[Theory]
		[InlineData(default, default)]
		[InlineData("", "")]
		[InlineData(" ", " ")]
		[InlineData(@"C:\code\temp\Helpers\Helpers.Tracing.Tests\ExtensionMethodsTests.cs", "ExtensionMethodsTests.cs")]
		public void ExtensionMethodsTests_ReducePath(string path, string expected)
		{
			Assert.Equal(
				expected,
				path.ReducePath());
		}

		[Theory]
		[InlineData(default, default, default)]
		[InlineData("test", default, "test")]
		[InlineData(default, "test", "test")]
		[InlineData(
			@"C:\code\temp\Helpers\Helpers.Tracing.Tests\ExtensionMethodsTests.cs",
			"ExtensionMethodsTests_BuildDefaultSpan",
			"ExtensionMethodsTests.cs=>ExtensionMethodsTests_BuildDefaultSpan")]
		public void ExtensionMethodsTests_BuildDefaultSpan(
			string filePath,
			string methodName,
			string expected)
		{
			// Arrange
			var tracerMock = new Mock<ITracer>();

			tracerMock
				.Setup(t => t.BuildSpan(It.IsAny<string>()))
				.Returns((string s) => new MockSpanBuilder(s));

			// Act
			var span = tracerMock.Object.BuildDefaultSpan(filePath, methodName);

			// Assert
			Assert.Equal(expected, ((MockSpanBuilder)span).OperationName);
		}

		private class MockSpanBuilder : ISpanBuilder
		{
			public MockSpanBuilder(string operationName)
			{
				OperationName = operationName;
			}

			public string OperationName { get; }

			public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext) => throw new NotImplementedException();
			public ISpanBuilder AsChildOf(ISpanContext parent) => throw new NotImplementedException();
			public ISpanBuilder AsChildOf(ISpan parent) => throw new NotImplementedException();
			public ISpanBuilder IgnoreActiveSpan() => throw new NotImplementedException();
			public ISpan Start() => throw new NotImplementedException();
			public IScope StartActive() => throw new NotImplementedException();
			public IScope StartActive(bool finishSpanOnDispose) => throw new NotImplementedException();
			public ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp) => throw new NotImplementedException();
			public ISpanBuilder WithTag(string key, string value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(string key, bool value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(string key, int value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(string key, double value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(BooleanTag tag, bool value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(IntOrStringTag tag, string value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(IntTag tag, int value) => throw new NotImplementedException();
			public ISpanBuilder WithTag(StringTag tag, string value) => throw new NotImplementedException();
		}
	}
}
