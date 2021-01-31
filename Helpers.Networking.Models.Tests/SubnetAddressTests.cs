using Xunit;

namespace Helpers.Networking.Models.Tests
{
	public class SubnetAddressTests
	{
		[Theory]
		[InlineData("127.0.0.1")]
		[InlineData("127.0.0.1/24")]
		public void Parse(string s) => SubnetAddress.Parse(s);

		[Theory]
		[InlineData("127.0.0.1")]
		[InlineData("127.0.0.1/24")]
		public void Equal(string s)
		{
			var o = SubnetAddress.Parse(s);
			Assert.Equal(o, o);
		}
	}
}
