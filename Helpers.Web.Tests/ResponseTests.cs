using Microsoft.Extensions.Primitives;
using System.Net;
using Xunit;

namespace Helpers.Web.Tests;

public class ResponseTests
{
	[Theory]
	[InlineData(default)]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("\t")]
	[InlineData("hello, world")]
	public void DeconstructionTests(string message)
	{
		// Arrange
		IReadOnlyDictionary<string, StringValues> headers = new Dictionary<string, StringValues>
		{
			["key"] = "value",
		};

		var statusCode = HttpStatusCode.OK;

		Models.IResponse<string> sut = new Models.Concrete.Response<string>(headers, statusCode, message);

		// Act
		var (actualHeaders, actualStatusCode, actualMessage) = sut;

		// Assert
		Assert.Equal(headers, actualHeaders);
		Assert.Equal(statusCode, actualStatusCode);
		Assert.Equal(message, actualMessage);
	}
}
