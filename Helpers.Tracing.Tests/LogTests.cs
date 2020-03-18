using Moq;
using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using Xunit;

namespace Helpers.Tracing.Tests
{
	public class LogTests
	{
		private readonly ISpan _span;
		private readonly IDictionary<string, object?> _logsDictionary = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);

		public LogTests()
		{
			var spanMock = new Mock<ISpan>(MockBehavior.Strict);

			spanMock
				.Setup(s => s.SetTag(It.IsAny<BooleanTag>(), It.IsAny<bool>()))
				.Returns(spanMock.Object);

			spanMock
				.Setup(s => s.Log(It.IsAny<IEnumerable<KeyValuePair<string, object?>>>()))
				.Callback<IEnumerable<KeyValuePair<string, object?>>>(kvps =>
				{
					foreach (var (key, value) in kvps)
					{
						_logsDictionary.Add(key, value);
					}
				})
				.Returns(spanMock.Object);

			_span = spanMock.Object;
		}

		[Theory]
		[InlineData("key", "value")]
		[InlineData("value", 0)]
		public void Params(string key, object value)
		{
			_logsDictionary.Clear();

			Assert.Empty(_logsDictionary);

			_span.Log(key, value);

			Assert.Single(_logsDictionary);
			Assert.True(_logsDictionary.ContainsKey(key));
			Assert.Equal(value, _logsDictionary[key]);
		}

		[Theory]
		[InlineData("key", "value")]
		[InlineData("value", 0)]
		public void TupleParams(string key, object value)
		{
			_logsDictionary.Clear();

			Assert.Empty(_logsDictionary);

			_span.Log((key, value));

			Assert.Single(_logsDictionary);
			Assert.True(_logsDictionary.ContainsKey(key));
			Assert.Equal(value, _logsDictionary[key]);
		}

		[Fact]
		public void Exception()
		{
			// Arrange
			_logsDictionary.Clear();

			var innerException = new InvalidTimeZoneException("inner")
			{
				Data =
				{
					["three"] = 3,
					["four"] = 4,
				},
			};

			var outerException = new InvalidOperationException("outer", innerException)
			{
				Data =
				{
					["one"] = 1,
					["two"] = 2,
				},
			};

			Assert.Empty(_logsDictionary);

			// Act
			_span.Log(outerException);

			// Assert
			Assert.NotEmpty(_logsDictionary);
			Assert.Contains(LogFields.ErrorKind, _logsDictionary.Keys);
			Assert.Contains(LogFields.ErrorObject, _logsDictionary.Keys);
			Assert.Contains(LogFields.Event, _logsDictionary.Keys);
			Assert.Contains(LogFields.Message, _logsDictionary.Keys);
			Assert.Contains(LogFields.Stack, _logsDictionary.Keys);
			Assert.Contains(nameof(System.Exception.Data), _logsDictionary.Keys);
			Assert.Equal("System.InvalidTimeZoneException", _logsDictionary[LogFields.ErrorKind]);
			Assert.IsType<InvalidTimeZoneException>(_logsDictionary[LogFields.ErrorObject]);
			Assert.Equal("error", _logsDictionary[LogFields.Event]);
			Assert.Equal("inner", _logsDictionary[LogFields.Message]);
			Assert.Equal("one=1;two=2;three=3;four=4;", _logsDictionary[nameof(System.Exception.Data)]);
		}

		[Theory]
		[InlineData("hello world", 1)]
		public void Object(string s, int i)
		{
			// Arrange
			_logsDictionary.Clear();

			var o = new Class { String = s, Int = i, };

			// Act
			_span.Log(o);

			// Assert
			Assert.NotEmpty(_logsDictionary);

			Assert.Contains(nameof(Class.String), _logsDictionary.Keys);
			Assert.Contains(nameof(Class.Int), _logsDictionary.Keys);

			Assert.NotNull(_logsDictionary[nameof(Class.String)]);
			Assert.NotNull(_logsDictionary[nameof(Class.Int)]);

			Assert.Equal(s, _logsDictionary[nameof(Class.String)]);
			Assert.Equal(i, _logsDictionary[nameof(Class.Int)]);
		}

		private class Class { public string? String { get; set; } public int? Int { get; set; } }
	}
}
