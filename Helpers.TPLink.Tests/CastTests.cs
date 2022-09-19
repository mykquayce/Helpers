using System.Net.NetworkInformation;

namespace Helpers.TPLink.Tests;

public class CastTests
{
	[Theory]
	[InlineData("alias", "device", "aaaaaaaaaaaa")]
	public void SystemInfoObject(string alias, string model, string physicalAddressString)
	{
		// Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		var before = new Models.Generated.ResponseObject.SystemObject.SystemInfoObject(default, default, model, default, default, default, default, default, default, alias, default, default, default, default, PhysicalAddress.Parse(physicalAddressString), default, default, default, default, default, default, default, default, default, default);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

		// Act
		var after = (Models.SystemInfo)before;

		// Assert
		Assert.Equal(alias, after.Alias);
		Assert.Equal(model, after.Model);
		Assert.Equal(physicalAddressString, after.PhysicalAddress.ToString().ToLowerInvariant());
	}
}
