using System.Text.Json;
using Xunit;

namespace Helpers.DockerHub.Tests;

public class ErrorTests
{
	[Theory]
	[InlineData(@"{""errors"":[{""code"":""UNAUTHORIZED"",""message"":""authentication required"",""detail"":[{""Type"":""repository"",""Class"":"""",""Name"":""pihole/pihole"",""Action"":""pull""}]}]}")]
	public void Deserialization(string json)
	{
		var o = JsonSerializer.Deserialize<ErrorResponseObject>(json)!;

		var actual = (Exception)o;
	}
}


[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public record ErrorResponseObject(IReadOnlyCollection<ErrorResponseObject.ErrorObject> errors)
{
	public static explicit operator Exception(ErrorResponseObject errorResponse)
		=> new AggregateException(errorResponse.errors.Select(e => (Exception)e));

	public record ErrorObject(string code, string message, IReadOnlyList<ErrorObject.ErrorDetailObject> detail)
	{
		public static explicit operator Exception(ErrorObject error)
		{
			var index = 0;
			var exception = new Exception($"{error.code} {error.message}");

			foreach (var (type, @class, name, action) in error.detail)
			{
				exception.Data.Add(nameof(type) + index, type);
				exception.Data.Add(nameof(@class) + index, @class);
				exception.Data.Add(nameof(name) + index, name);
				exception.Data.Add(nameof(action) + index, action);
				index++;
			}

			return exception;
		}

		public record ErrorDetailObject(string Type, string Class, string Name, string Action);
	}
}
