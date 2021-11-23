using Dawn;
using Xunit;

namespace Helpers.DockerHub.Tests;

public class DawnGuardTests
{
	[Theory]
	[InlineData("eassbhhtgu/asnblacklister")]
	[InlineData("eassbhhtgu/elgatoapi")]
	[InlineData("eassbhhtgu/networkdiscoveryapi")]
	[InlineData("eassbhhtgu/networktest")]
	[InlineData("eassbhhtgu/pihole")]
	[InlineData("eassbhhtgu/randomwebbrowsing")]
	[InlineData("eassbhhtgu/yarp")]
	[InlineData("loicsharma/baget")]
	[InlineData("mariadb")]
	[InlineData("mcr.microsoft.com/dotnet/aspnet")]
	[InlineData("mcr.microsoft.com/dotnet/runtime")]
	[InlineData("mcr.microsoft.com/dotnet/sdk")]
	[InlineData("pihole/pihole")]
	[InlineData("portainer/portainer-ce")]
	[InlineData("rabbitmq")]
	[InlineData("ubuntu")]
	public void IsTagNameTests(string image) => Guard.Argument(image).IsTagName();
}
