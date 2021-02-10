using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

	public class ArpResultsCollectionTests
	{

		[Theory]
		[InlineData(@"
Interface: 192.168.1.50 --- 0x8
  Internet Address      Physical Address      Type
  192.168.1.2           80-13-82-96-e9-f2     dynamic   
  192.168.1.10          32-23-03-20-d7-96     dynamic   
  192.168.1.60          94-c6-91-17-7a-11     dynamic   
  192.168.1.70          ac-22-0b-4f-d4-13     dynamic   
  192.168.1.101         00-15-5d-01-da-00     dynamic   
  192.168.1.102         00-15-5d-01-da-00     dynamic   
  192.168.1.133         00-c2-c6-cb-35-31     dynamic   
  192.168.1.198         90-78-41-1c-47-ab     dynamic   
  192.168.1.214         7e-a7-b0-32-8b-3e     dynamic   
  192.168.1.217         3c-6a-9d-14-d7-65     dynamic   
  192.168.1.218         9c-5c-8e-bd-e5-d6     dynamic   
  192.168.1.255         ff-ff-ff-ff-ff-ff     static    
  224.0.0.2             01-00-5e-00-00-02     static    
  224.0.0.22            01-00-5e-00-00-16     static    
  224.0.0.251           01-00-5e-00-00-fb     static    
  224.0.0.252           01-00-5e-00-00-fc     static    
  239.255.250.250       01-00-5e-7f-fa-fa     static    
  239.255.255;.250       01-00-5e-7f-ff-fa     static    
", "192.168.1.50")]
		[InlineData(@"
Interface: 192.168.1.218 --- 0xf
  Internet Address      Physical Address      Type
  192.168.1.2           80-13-82-96-e9-f2     dynamic
  192.168.1.10          32-23-03-20-d7-96     dynamic
  192.168.1.50          c0-3f-d5-67-7c-da     dynamic
  192.168.1.60          94-c6-91-17-7a-11     dynamic
  192.168.1.156         ec-b5-fa-18-e3-24     dynamic
  192.168.1.217         3c-6a-9d-14-d7-65     dynamic
  192.168.1.230         f0-6e-0b-40-90-4d     dynamic
  192.168.1.255         ff-ff-ff-ff-ff-ff     static
  224.0.0.2             01-00-5e-00-00-02     static
  224.0.0.22            01-00-5e-00-00-16     static
  224.0.0.251           01-00-5e-00-00-fb     static
  224.0.0.252           01-00-5e-00-00-fc     static
  239.255.255.250       01-00-5e-7f-ff-fa     static
  255.255.255.255       ff-ff-ff-ff-ff-ff     static

Interface: 172.25.224.1 --- 0x22
  Internet Address      Physical Address      Type
  172.25.239.255        ff-ff-ff-ff-ff-ff     static
  224.0.0.2             01-00-5e-00-00-02     static
  224.0.0.22            01-00-5e-00-00-16     static
  224.0.0.251           01-00-5e-00-00-fb     static
  239.255.255.250       01-00-5e-7f-ff-fa     static
  255.255.255.255       ff-ff-ff-ff-ff-ff     static

Interface: 172.22.192.1 --- 0x2b
  Internet Address      Physical Address      Type
  172.22.207.255        ff-ff-ff-ff-ff-ff     static
  224.0.0.2             01-00-5e-00-00-02     static
  224.0.0.22            01-00-5e-00-00-16     static
  224.0.0.251           01-00-5e-00-00-fb     static
  239.255.255.250       01-00-5e-7f-ff-fa     static", "192.168.1.218", "172.25.224.1", "172.22.192.1")]
		public void Parse(string input, params string[] ipAddressStrings)
		{
			var actual = ArpResultsDictionary.Parse(input);

			Assert.NotNull(actual);
			Assert.NotEmpty(actual);
			Assert.Equal(ipAddressStrings.Length, actual.Count);
		}
	}
}
