using Microsoft.AspNetCore.Http;
using Moq;
using OpenTracing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Tracing.Middleware.Tests
{
	public class TraceMiddlewareTests
	{
		[Theory]
		[InlineData("")]
		[InlineData("a")]
		[InlineData("neptrsfdznhetprdsfnehdtrpsnhek,brtdpsfzvne,hkrtsfpzne,hbrtsfhnepz,lbvtspfzhbne,lrtfhpsnel,btpsfrhelnj,")]
		public void TraceMiddlewareTests_AttachRequestBodyToTraceMiddlewareTests_InvokeAsync_SavesTheBody(string body)
		{
			// Arrange
			var results = new List<string>();
			var requestDelegate = Mock.Of<RequestDelegate>(rd => rd.Invoke(It.IsAny<HttpContext>()) == Task.CompletedTask);

			var spanMock = new Mock<ISpan>(MockBehavior.Strict);

			spanMock
				.Setup(span => span.SetTag(It.Is<string>(key => key == "http.body"), It.IsAny<string>()))
				.Callback<string, string>((key, value) => results.Add(value))
				.Returns<string, string>((key, value) => spanMock.Object);

			var tracer = Mock.Of<ITracer>(t => t.ActiveSpan == spanMock.Object);

			var sut = new AttachRequestBodyToTraceMiddleware(requestDelegate, tracer);

			var bytes = Encoding.UTF8.GetBytes(body);
			using var stream = new MemoryStream(bytes);

			var httpContext = Mock.Of<HttpContext>(c => c.Request.Body == stream);

			// Act
			sut.InvokeAsync(httpContext).GetAwaiter().GetResult();

			// Assert
			spanMock
				.Verify(span => span.SetTag(It.Is<string>(key => key == "http.body"), It.IsAny<string>()), Times.Once);

			Assert.Single(results);
			Assert.Equal(body, results[0]);
		}

		[Theory]
		[InlineData("")]
		[InlineData("a")]
		[InlineData("neptrsfdznhetprdsfnehdtrpsnhek,brtdpsfzvne,hkrtsfpzne,hbrtsfhnepz,lbvtspfzhbne,lrtfhpsnel,btpsfrhelnj,")]
		public void TraceMiddlewareTests_AttachResponseBodyToTraceMiddlewareTests_InvokeAsync_SavesTheBody(string body)
		{
			// Arrange
			var results = new List<string>();
			var requestDelegate = Mock.Of<RequestDelegate>(rd => rd.Invoke(It.IsAny<HttpContext>()) == Task.CompletedTask);

			var spanMock = new Mock<ISpan>(MockBehavior.Strict);

			spanMock
				.Setup(span => span.SetTag(It.Is<string>(key => key == "http.body"), It.IsAny<string>()))
				.Callback<string, string>((key, value) => results.Add(value))
				.Returns<string, string>((key, value) => spanMock.Object);

			var tracer = Mock.Of<ITracer>(t => t.ActiveSpan == spanMock.Object);

			var sut = new AttachResponseBodyToTraceMiddleware(requestDelegate, tracer);

			var bytes = Encoding.UTF8.GetBytes(body);
			using var stream = new MemoryStream(bytes);

			var httpContext = Mock.Of<HttpContext>(c => c.Response.Body == stream);

			// Act
			sut.InvokeAsync(httpContext).GetAwaiter().GetResult();

			// Assert
			spanMock
				.Verify(span => span.SetTag(It.Is<string>(key => key == "http.body"), It.IsAny<string>()), Times.Once);

			Assert.Single(results);
			Assert.Equal(body, results[0]);
		}
	}
}
