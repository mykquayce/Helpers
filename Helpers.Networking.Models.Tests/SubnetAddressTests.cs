﻿using System.Collections.Generic;
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
		[InlineData("31.13.24.0/21")]
		[InlineData("31.13.64.0/18")]
		[InlineData("31.13.64.0/19")]
		[InlineData("31.13.64.0/24")]
		[InlineData("31.13.65.0/24")]
		[InlineData("31.13.66.0/24")]
		[InlineData("31.13.67.0/24")]
		[InlineData("31.13.68.0/24")]
		[InlineData("31.13.69.0/24")]
		[InlineData("31.13.70.0/24")]
		[InlineData("31.13.71.0/24")]
		[InlineData("31.13.72.0/24")]
		[InlineData("31.13.73.0/24")]
		[InlineData("31.13.74.0/24")]
		[InlineData("31.13.75.0/24")]
		[InlineData("31.13.76.0/24")]
		[InlineData("31.13.77.0/24")]
		[InlineData("31.13.80.0/24")]
		[InlineData("31.13.81.0/24")]
		[InlineData("31.13.82.0/24")]
		[InlineData("31.13.83.0/24")]
		[InlineData("31.13.84.0/24")]
		[InlineData("31.13.85.0/24")]
		[InlineData("31.13.86.0/24")]
		[InlineData("31.13.87.0/24")]
		[InlineData("31.13.88.0/24")]
		[InlineData("31.13.89.0/24")]
		[InlineData("31.13.92.0/24")]
		[InlineData("31.13.93.0/24")]
		[InlineData("31.13.94.0/24")]
		[InlineData("31.13.96.0/19")]
		[InlineData("45.64.40.0/22")]
		[InlineData("66.220.144.0/20")]
		[InlineData("66.220.144.0/21")]
		[InlineData("66.220.152.0/21")]
		[InlineData("69.63.176.0/20")]
		[InlineData("69.63.176.0/21")]
		[InlineData("69.63.184.0/21")]
		[InlineData("69.171.224.0/19")]
		[InlineData("69.171.224.0/20")]
		[InlineData("69.171.240.0/20")]
		[InlineData("69.171.250.0/24")]
		[InlineData("74.119.76.0/22")]
		[InlineData("102.132.96.0/20")]
		[InlineData("102.132.96.0/24")]
		[InlineData("102.132.99.0/24")]
		[InlineData("103.4.96.0/22")]
		[InlineData("129.134.0.0/17")]
		[InlineData("129.134.25.0/24")]
		[InlineData("129.134.26.0/24")]
		[InlineData("129.134.27.0/24")]
		[InlineData("129.134.28.0/24")]
		[InlineData("129.134.29.0/24")]
		[InlineData("129.134.30.0/23")]
		[InlineData("129.134.30.0/24")]
		[InlineData("129.134.31.0/24")]
		[InlineData("129.134.64.0/24")]
		[InlineData("129.134.65.0/24")]
		[InlineData("129.134.66.0/24")]
		[InlineData("129.134.67.0/24")]
		[InlineData("129.134.68.0/24")]
		[InlineData("129.134.69.0/24")]
		[InlineData("129.134.70.0/24")]
		[InlineData("129.134.71.0/24")]
		[InlineData("129.134.72.0/24")]
		[InlineData("129.134.73.0/24")]
		[InlineData("129.134.74.0/24")]
		[InlineData("129.134.127.0/24")]
		[InlineData("157.240.0.0/17")]
		[InlineData("157.240.0.0/24")]
		[InlineData("157.240.1.0/24")]
		[InlineData("157.240.2.0/24")]
		[InlineData("157.240.3.0/24")]
		[InlineData("157.240.6.0/24")]
		[InlineData("157.240.7.0/24")]
		[InlineData("157.240.8.0/24")]
		[InlineData("157.240.9.0/24")]
		[InlineData("157.240.10.0/24")]
		[InlineData("157.240.11.0/24")]
		[InlineData("157.240.12.0/24")]
		[InlineData("157.240.13.0/24")]
		[InlineData("157.240.14.0/24")]
		[InlineData("157.240.15.0/24")]
		[InlineData("157.240.17.0/24")]
		[InlineData("157.240.18.0/24")]
		[InlineData("157.240.19.0/24")]
		[InlineData("157.240.20.0/24")]
		[InlineData("157.240.21.0/24")]
		[InlineData("157.240.22.0/24")]
		[InlineData("157.240.24.0/24")]
		[InlineData("157.240.26.0/24")]
		[InlineData("157.240.27.0/24")]
		[InlineData("157.240.28.0/24")]
		[InlineData("157.240.29.0/24")]
		[InlineData("157.240.30.0/24")]
		[InlineData("157.240.31.0/24")]
		[InlineData("157.240.192.0/18")]
		[InlineData("157.240.193.0/24")]
		[InlineData("157.240.194.0/24")]
		[InlineData("157.240.195.0/24")]
		[InlineData("157.240.196.0/24")]
		[InlineData("157.240.197.0/24")]
		[InlineData("157.240.199.0/24")]
		[InlineData("157.240.200.0/24")]
		[InlineData("157.240.201.0/24")]
		[InlineData("157.240.204.0/24")]
		[InlineData("157.240.205.0/24")]
		[InlineData("157.240.206.0/24")]
		[InlineData("157.240.209.0/24")]
		[InlineData("157.240.210.0/24")]
		[InlineData("157.240.211.0/24")]
		[InlineData("157.240.212.0/24")]
		[InlineData("157.240.213.0/24")]
		[InlineData("157.240.214.0/24")]
		[InlineData("157.240.215.0/24")]
		[InlineData("157.240.216.0/24")]
		[InlineData("157.240.217.0/24")]
		[InlineData("157.240.218.0/24")]
		[InlineData("157.240.219.0/24")]
		[InlineData("157.240.220.0/24")]
		[InlineData("157.240.221.0/24")]
		[InlineData("157.240.222.0/24")]
		[InlineData("157.240.223.0/24")]
		[InlineData("157.240.224.0/24")]
		[InlineData("173.252.64.0/19")]
		[InlineData("173.252.88.0/21")]
		[InlineData("173.252.96.0/19")]
		[InlineData("179.60.192.0/22")]
		[InlineData("179.60.192.0/24")]
		[InlineData("179.60.194.0/24")]
		[InlineData("179.60.195.0/24")]
		[InlineData("185.60.216.0/22")]
		[InlineData("185.60.216.0/24")]
		[InlineData("185.60.217.0/24")]
		[InlineData("185.60.218.0/24")]
		[InlineData("185.60.219.0/24")]
		[InlineData("185.89.218.0/23")]
		[InlineData("185.89.218.0/24")]
		[InlineData("185.89.219.0/24")]
		[InlineData("204.15.20.0/22")]
		[InlineData("2620:0:1c00::/40")]
		[InlineData("2a03:2880::/32")]
		[InlineData("2a03:2880::/36")]
		[InlineData("2a03:2880:1000::/36")]
		[InlineData("2a03:2880:2000::/36")]
		[InlineData("2a03:2880:3000::/36")]
		[InlineData("2a03:2880:f001::/48")]
		[InlineData("2a03:2880:f003::/48")]
		[InlineData("2a03:2880:f004::/48")]
		[InlineData("2a03:2880:f005::/48")]
		[InlineData("2a03:2880:f006::/48")]
		[InlineData("2a03:2880:f007::/48")]
		[InlineData("2a03:2880:f008::/48")]
		[InlineData("2a03:2880:f00a::/48")]
		[InlineData("2a03:2880:f00c::/48")]
		[InlineData("2a03:2880:f00d::/48")]
		[InlineData("2a03:2880:f00e::/48")]
		[InlineData("2a03:2880:f00f::/48")]
		[InlineData("2a03:2880:f010::/48")]
		[InlineData("2a03:2880:f011::/48")]
		[InlineData("2a03:2880:f012::/48")]
		[InlineData("2a03:2880:f013::/48")]
		[InlineData("2a03:2880:f016::/48")]
		[InlineData("2a03:2880:f017::/48")]
		[InlineData("2a03:2880:f019::/48")]
		[InlineData("2a03:2880:f01c::/48")]
		[InlineData("2a03:2880:f01f::/48")]
		[InlineData("2a03:2880:f021::/48")]
		[InlineData("2a03:2880:f023::/48")]
		[InlineData("2a03:2880:f024::/48")]
		[InlineData("2a03:2880:f027::/48")]
		[InlineData("2a03:2880:f028::/48")]
		[InlineData("2a03:2880:f029::/48")]
		[InlineData("2a03:2880:f02a::/48")]
		[InlineData("2a03:2880:f02b::/48")]
		[InlineData("2a03:2880:f02c::/48")]
		[InlineData("2a03:2880:f02d::/48")]
		[InlineData("2a03:2880:f02f::/48")]
		[InlineData("2a03:2880:f030::/48")]
		[InlineData("2a03:2880:f031::/48")]
		[InlineData("2a03:2880:f032::/48")]
		[InlineData("2a03:2880:f033::/48")]
		[InlineData("2a03:2880:f034::/48")]
		[InlineData("2a03:2880:f035::/48")]
		[InlineData("2a03:2880:f036::/48")]
		[InlineData("2a03:2880:f037::/48")]
		[InlineData("2a03:2880:f038::/48")]
		[InlineData("2a03:2880:f03a::/48")]
		[InlineData("2a03:2880:f03d::/48")]
		[InlineData("2a03:2880:f03e::/48")]
		[InlineData("2a03:2880:f03f::/48")]
		[InlineData("2a03:2880:f040::/48")]
		[InlineData("2a03:2880:f041::/48")]
		[InlineData("2a03:2880:f042::/48")]
		[InlineData("2a03:2880:f043::/48")]
		[InlineData("2a03:2880:f044::/48")]
		[InlineData("2a03:2880:f045::/48")]
		[InlineData("2a03:2880:f047::/48")]
		[InlineData("2a03:2880:f048::/48")]
		[InlineData("2a03:2880:f04a::/48")]
		[InlineData("2a03:2880:f04b::/48")]
		[InlineData("2a03:2880:f04e::/48")]
		[InlineData("2a03:2880:f04f::/48")]
		[InlineData("2a03:2880:f050::/48")]
		[InlineData("2a03:2880:f052::/48")]
		[InlineData("2a03:2880:f053::/48")]
		[InlineData("2a03:2880:f054::/48")]
		[InlineData("2a03:2880:f056::/48")]
		[InlineData("2a03:2880:f057::/48")]
		[InlineData("2a03:2880:f058::/48")]
		[InlineData("2a03:2880:f059::/48")]
		[InlineData("2a03:2880:f05a::/48")]
		[InlineData("2a03:2880:f05b::/48")]
		[InlineData("2a03:2880:f05c::/48")]
		[InlineData("2a03:2880:f05e::/48")]
		[InlineData("2a03:2880:f060::/48")]
		[InlineData("2a03:2880:f065::/48")]
		[InlineData("2a03:2880:f066::/48")]
		[InlineData("2a03:2880:f067::/48")]
		[InlineData("2a03:2880:f068::/48")]
		[InlineData("2a03:2880:f0fc::/47")]
		[InlineData("2a03:2880:f0fc::/48")]
		[InlineData("2a03:2880:f0fd::/48")]
		[InlineData("2a03:2880:f0ff::/48")]
		[InlineData("2a03:2880:f101::/48")]
		[InlineData("2a03:2880:f103::/48")]
		[InlineData("2a03:2880:f104::/48")]
		[InlineData("2a03:2880:f105::/48")]
		[InlineData("2a03:2880:f106::/48")]
		[InlineData("2a03:2880:f107::/48")]
		[InlineData("2a03:2880:f108::/48")]
		[InlineData("2a03:2880:f10a::/48")]
		[InlineData("2a03:2880:f10c::/48")]
		[InlineData("2a03:2880:f10d::/48")]
		[InlineData("2a03:2880:f10e::/48")]
		[InlineData("2a03:2880:f10f::/48")]
		[InlineData("2a03:2880:f110::/48")]
		[InlineData("2a03:2880:f111::/48")]
		[InlineData("2a03:2880:f112::/48")]
		[InlineData("2a03:2880:f113::/48")]
		[InlineData("2a03:2880:f116::/48")]
		[InlineData("2a03:2880:f117::/48")]
		[InlineData("2a03:2880:f119::/48")]
		[InlineData("2a03:2880:f11c::/48")]
		[InlineData("2a03:2880:f11f::/48")]
		[InlineData("2a03:2880:f121::/48")]
		[InlineData("2a03:2880:f123::/48")]
		[InlineData("2a03:2880:f124::/48")]
		[InlineData("2a03:2880:f127::/48")]
		[InlineData("2a03:2880:f128::/48")]
		[InlineData("2a03:2880:f129::/48")]
		[InlineData("2a03:2880:f12a::/48")]
		[InlineData("2a03:2880:f12b::/48")]
		[InlineData("2a03:2880:f12c::/48")]
		[InlineData("2a03:2880:f12d::/48")]
		[InlineData("2a03:2880:f12f::/48")]
		[InlineData("2a03:2880:f130::/48")]
		[InlineData("2a03:2880:f131::/48")]
		[InlineData("2a03:2880:f132::/48")]
		[InlineData("2a03:2880:f133::/48")]
		[InlineData("2a03:2880:f134::/48")]
		[InlineData("2a03:2880:f135::/48")]
		[InlineData("2a03:2880:f136::/48")]
		[InlineData("2a03:2880:f137::/48")]
		[InlineData("2a03:2880:f138::/48")]
		[InlineData("2a03:2880:f13a::/48")]
		[InlineData("2a03:2880:f13d::/48")]
		[InlineData("2a03:2880:f13e::/48")]
		[InlineData("2a03:2880:f13f::/48")]
		[InlineData("2a03:2880:f140::/48")]
		[InlineData("2a03:2880:f141::/48")]
		[InlineData("2a03:2880:f142::/48")]
		[InlineData("2a03:2880:f143::/48")]
		[InlineData("2a03:2880:f144::/48")]
		[InlineData("2a03:2880:f145::/48")]
		[InlineData("2a03:2880:f147::/48")]
		[InlineData("2a03:2880:f148::/48")]
		[InlineData("2a03:2880:f14a::/48")]
		[InlineData("2a03:2880:f14b::/48")]
		[InlineData("2a03:2880:f14e::/48")]
		[InlineData("2a03:2880:f14f::/48")]
		[InlineData("2a03:2880:f150::/48")]
		[InlineData("2a03:2880:f152::/48")]
		[InlineData("2a03:2880:f153::/48")]
		[InlineData("2a03:2880:f154::/48")]
		[InlineData("2a03:2880:f156::/48")]
		[InlineData("2a03:2880:f157::/48")]
		[InlineData("2a03:2880:f158::/48")]
		[InlineData("2a03:2880:f159::/48")]
		[InlineData("2a03:2880:f15a::/48")]
		[InlineData("2a03:2880:f15b::/48")]
		[InlineData("2a03:2880:f15c::/48")]
		[InlineData("2a03:2880:f15e::/48")]
		[InlineData("2a03:2880:f160::/48")]
		[InlineData("2a03:2880:f162::/48")]
		[InlineData("2a03:2880:f163::/48")]
		[InlineData("2a03:2880:f164::/48")]
		[InlineData("2a03:2880:f165::/48")]
		[InlineData("2a03:2880:f1fc::/47")]
		[InlineData("2a03:2880:f1fc::/48")]
		[InlineData("2a03:2880:f1fd::/48")]
		[InlineData("2a03:2880:f1ff::/48")]
		[InlineData("2a03:2880:f201::/48")]
		[InlineData("2a03:2880:f203::/48")]
		[InlineData("2a03:2880:f204::/48")]
		[InlineData("2a03:2880:f205::/48")]
		[InlineData("2a03:2880:f206::/48")]
		[InlineData("2a03:2880:f207::/48")]
		[InlineData("2a03:2880:f208::/48")]
		[InlineData("2a03:2880:f20a::/48")]
		[InlineData("2a03:2880:f20c::/48")]
		[InlineData("2a03:2880:f20d::/48")]
		[InlineData("2a03:2880:f20e::/48")]
		[InlineData("2a03:2880:f20f::/48")]
		[InlineData("2a03:2880:f210::/48")]
		[InlineData("2a03:2880:f211::/48")]
		[InlineData("2a03:2880:f212::/48")]
		[InlineData("2a03:2880:f213::/48")]
		[InlineData("2a03:2880:f216::/48")]
		[InlineData("2a03:2880:f217::/48")]
		[InlineData("2a03:2880:f219::/48")]
		[InlineData("2a03:2880:f21c::/48")]
		[InlineData("2a03:2880:f21f::/48")]
		[InlineData("2a03:2880:f221::/48")]
		[InlineData("2a03:2880:f223::/48")]
		[InlineData("2a03:2880:f224::/48")]
		[InlineData("2a03:2880:f227::/48")]
		[InlineData("2a03:2880:f228::/48")]
		[InlineData("2a03:2880:f229::/48")]
		[InlineData("2a03:2880:f22a::/48")]
		[InlineData("2a03:2880:f22b::/48")]
		[InlineData("2a03:2880:f22c::/48")]
		[InlineData("2a03:2880:f22d::/48")]
		[InlineData("2a03:2880:f22f::/48")]
		[InlineData("2a03:2880:f230::/48")]
		[InlineData("2a03:2880:f231::/48")]
		[InlineData("2a03:2880:f232::/48")]
		[InlineData("2a03:2880:f233::/48")]
		[InlineData("2a03:2880:f234::/48")]
		[InlineData("2a03:2880:f235::/48")]
		[InlineData("2a03:2880:f236::/48")]
		[InlineData("2a03:2880:f237::/48")]
		[InlineData("2a03:2880:f238::/48")]
		[InlineData("2a03:2880:f23a::/48")]
		[InlineData("2a03:2880:f23d::/48")]
		[InlineData("2a03:2880:f23e::/48")]
		[InlineData("2a03:2880:f23f::/48")]
		[InlineData("2a03:2880:f240::/48")]
		[InlineData("2a03:2880:f241::/48")]
		[InlineData("2a03:2880:f242::/48")]
		[InlineData("2a03:2880:f243::/48")]
		[InlineData("2a03:2880:f244::/48")]
		[InlineData("2a03:2880:f245::/48")]
		[InlineData("2a03:2880:f247::/48")]
		[InlineData("2a03:2880:f248::/48")]
		[InlineData("2a03:2880:f24a::/48")]
		[InlineData("2a03:2880:f24b::/48")]
		[InlineData("2a03:2880:f24e::/48")]
		[InlineData("2a03:2880:f24f::/48")]
		[InlineData("2a03:2880:f250::/48")]
		[InlineData("2a03:2880:f252::/48")]
		[InlineData("2a03:2880:f253::/48")]
		[InlineData("2a03:2880:f254::/48")]
		[InlineData("2a03:2880:f256::/48")]
		[InlineData("2a03:2880:f257::/48")]
		[InlineData("2a03:2880:f258::/48")]
		[InlineData("2a03:2880:f259::/48")]
		[InlineData("2a03:2880:f25a::/48")]
		[InlineData("2a03:2880:f25b::/48")]
		[InlineData("2a03:2880:f25c::/48")]
		[InlineData("2a03:2880:f25e::/48")]
		[InlineData("2a03:2880:f260::/48")]
		[InlineData("2a03:2880:f262::/48")]
		[InlineData("2a03:2880:f263::/48")]
		[InlineData("2a03:2880:f264::/48")]
		[InlineData("2a03:2880:f265::/48")]
		[InlineData("2a03:2880:f2ff::/48")]
		[InlineData("2a03:2880:ff08::/48")]
		[InlineData("2a03:2880:ff09::/48")]
		[InlineData("2a03:2880:ff0a::/48")]
		[InlineData("2a03:2880:ff0b::/48")]
		[InlineData("2a03:2880:ff0c::/48")]
		[InlineData("2a03:2881::/32")]
		[InlineData("2a03:2881::/48")]
		[InlineData("2a03:2881:1::/48")]
		[InlineData("2a03:2881:2::/48")]
		[InlineData("2a03:2881:3::/48")]
		[InlineData("2a03:2881:4::/48")]
		[InlineData("2a03:2881:5::/48")]
		[InlineData("2a03:2881:6::/48")]
		[InlineData("2a03:2881:7::/48")]
		[InlineData("2a03:2881:8::/48")]
		[InlineData("2a03:2881:9::/48")]
		[InlineData("2a03:2881:a::/48")]
		[InlineData("2a03:2881:4000::/48")]
		[InlineData("2a03:2881:4001::/48")]
		[InlineData("2a03:2881:4002::/48")]
		[InlineData("2a03:2881:4003::/48")]
		[InlineData("2a03:2881:4004::/48")]
		[InlineData("2a03:2881:4006::/48")]
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
