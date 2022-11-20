using System.Text.Json;

namespace Helpers.TPLink.Tests;

public class SerializationTests
{
	[Fact]
	public void Test1()
	{
		var o = new { system = new { get_sysinfo = new { }, }, emeter = new { get_realtime = new { }, }, };

		var bytes = o.Serialize().Encode().Encrypt();

		var s = string.Join(' ', from b in bytes
								 select b.ToString("x").PadLeft(2, '0'));
	}

	[Theory]
	//[InlineData("d0 f2 81 f8 8b ff 9a f7 d5 ef 94 b6 d1 b4 c0 9f ec 95 e6 8f e1 87 e8 ca f0 8b a9 da ad f2 84 e1 93 b1 8b a9 98 b6 86 a8 99 af 8f cd b8 d1 bd d9 f9 cb fa ca f8 c8 fd dd 8f ea 86 a8 99 af 9c ab 98 ad 8f a3 81 e9 9e c1 b7 d2 a0 82 b8 9a ab 85 b5 97 bb 99 f4 9b ff 9a f6 d4 ee cc 87 d7 e6 d7 e2 ca 9f d4 fd df f3 d1 b5 d0 a6 cf ac c9 80 e4 c6 fc de e6 d6 e6 d0 e4 a1 e7 d6 e7 d4 e7 d0 e8 ab 99 df 9a db e9 ab 9e df 9d db e3 d2 eb de 9d ab 9e db ea af 9e ae 99 ad 99 a8 8a a6 84 eb 8e e3 aa ce ec d6 f4 b7 80 c1 f2 c4 81 b1 f2 c0 84 b0 f2 b3 f1 c5 f1 b5 f0 b4 82 c7 81 b1 89 be 8e cf 8c bb 8b bc fa d8 f4 d6 be c9 80 e4 c6 fc de ed d4 91 a9 9d ad 95 d0 94 ad 9a ae ea ae 98 a1 e5 dd 9c ab 9c d8 e1 a7 9f a8 90 a1 97 a4 93 d6 f4 d8 fa 88 fb 88 e1 c3 f9 d4 e3 d3 ff dd b1 d0 a4 cd b9 cc a8 cd 92 fb d9 e3 d6 e5 d0 e3 d3 e0 cc ee 82 ed 83 e4 8d f9 8c e8 8d d2 bb 99 a3 8e bc 8d b9 89 b1 9d bf de b2 db ba c9 eb d1 f3 85 f7 d7 b1 c3 ac c2 b6 94 b8 9a e9 9d fc 88 fd 8e ac 96 b4 d7 b8 d6 b0 d9 be cb b9 dc b8 9a b6 94 fb 99 fd a2 d1 a3 c0 e2 d8 fa 8e fe 92 fb 95 fe dc f0 d2 bf d6 b5 ea 9e e7 97 f2 d0 ea c8 81 ce 9a b4 e7 aa eb b9 ed bd f1 a4 e3 b0 e7 ae fa b9 f1 d3 ff dd bb de bf cb be cc a9 8b b1 93 c7 8e c3 f9 bc f2 b7 95 b9 9b f6 97 f4 d6 ec ce fc c4 fe bb fe c4 f1 c3 f9 bc fe c4 f4 b5 8f ce fa d8 f4 d6 a3 d3 b7 d6 a2 cb a5 c2 e0 da ea c6 e4 88 ed 89 d6 b9 df b9 9b a1 91 bd 9f ed 88 e4 85 fc a3 d0 a4 c5 b1 d4 f6 cc fc d0 f2 9d f3 ac d8 b1 dc b9 9b a1 91 bd 9f f6 95 fa 94 cb a3 c2 b1 d9 fb c1 e3 c1 ed cf ab ce b8 e7 89 e8 85 e0 c2 f8 da 89 e4 85 f7 83 a3 f4 9d b0 f6 9f bf ef 83 f6 91 b1 fc 95 fb 92 b0 9c be df bc c8 a1 d7 b2 ed 80 ef 8b ee cc f6 d4 ba d5 bb de fc d0 f2 9c f9 81 f5 aa cb a8 dc b5 da b4 96 ac d7 f5 81 f8 88 ed cf f5 d8 e9 94 b8 9a f4 80 e3 bc cf bb da ae cb e9 d3 e3 cf ed 88 fa 88 d7 b4 db bf da f8 c2 f2 8f f2 de fc 99 f4 91 e5 80 f2 d0 ea 91 b3 d4 b1 c5 9a e8 8d ec 80 f4 9d f0 95 b7 8d f6 d4 b7 c2 b0 c2 a7 c9 bd e2 8f ee cc f6 c6 ea c8 be d1 bd c9 a8 cf aa f5 98 ee cc f6 c4 f1 c1 f2 c6 f1 dd ff 8f e0 97 f2 80 df b2 c5 e7 dd ed c1 e3 97 f8 8c ed 81 de a9 c1 e3 d9 ea d2 fe dc b9 cb b9 e6 85 ea 8e eb c9 f3 c3 be c3 be")]
	//[InlineData("d0 f2 81 f8 8b ff 9a f7 d5 ef 94 b6 d1 b4 c0 9f ec 95 e6 8f e1 87 e8 ca f0 8b a9 da ad f2 84 e1 93 b1 8b a9 98 b6 86 a8 99 af 8f cd b8 d1 bd d9 f9 cb fa ca f8 c8 fd dd 8f ea 86 a8 99 af 9c ab 98 ad 8f a3 81 e9 9e c1 b7 d2 a0 82 b8 9a ab 85 b5 97 bb 99 f4 9b ff 9a f6 d4 ee cc 87 d7 e6 d7 e2 ca 9f d4 fd df f3 d1 b5 d0 a6 cf ac c9 80 e4 c6 fc de e6 d6 e6 d0 94 a0 95 d4 e3 d3 e2 d1 93 d5 96 a1 96 a0 91 d4 e4 a1 96 af ee ad 9f da ef d6 e2 da eb ae 9c ab 92 a0 e5 d0 f2 de fc 93 f6 9b d2 b6 94 ae 8c cf f8 b9 8a bc f9 c9 8a b8 fc c8 8a cb 89 bd 89 cd 88 cc fa bf f9 c9 f1 c6 f6 b7 f4 c3 f3 c4 82 a0 8c ae c6 b1 f8 9c be 84 a6 95 ac e9 d1 e5 d5 ed a8 ec d5 e2 d6 92 d6 e0 d9 9d a5 e4 d3 e4 a0 99 df e7 d0 e8 d9 ef dc eb ae 8c a0 82 f0 83 f0 99 bb 81 ac 9a ad 81 a3 cf ae da b3 c7 b2 d6 b3 ec 85 a7 9d a8 9b ae 9d ad 9e b2 90 fc 93 fd 9a f3 87 f2 96 f3 ac c5 e7 dd f0 c2 f3 c7 f7 cf e3 c1 a0 cc a5 c4 b7 95 af 8d ec 81 f1 d3 ff dd ae da bb cf ba c9 eb d1 f3 90 ff 91 f7 9e f9 8c fe 9b ff dd f1 d3 bc de ba e5 96 e4 87 a5 9f bd c9 b9 d5 bc d2 b9 9b b7 95 f8 91 f2 ad d9 a0 d0 b5 97 ad 8f c6 89 dd f3 a0 ed ac fe aa fa b6 e3 a4 f7 a0 e9 bd fe b6 94 b8 9a fc 99 f8 8c f9 8b ee cc f6 d4 80 c9 84 be fb b5 f0 d2 fe dc b1 d0 b3 91 ab 89 b9 89 b3 80 b1 8b b2 80 ba ff ce f4 b5 83 b9 81 c3 e1 cd ef 9a ea 8e ef 9b f2 9c fb d9 e3 d3 ff dd b1 d4 b0 ef 80 e6 80 a2 98 a8 84 a6 d4 b1 dd bc c5 9a e9 9d fc 88 ed cf f5 c4 e8 ca a5 cb 94 e0 89 e4 81 a3 99 ac 9a a3 8f ad c4 a7 c8 a6 f9 91 f0 83 eb c9 f3 d1 f3 df fd 99 fc 8a d5 bb da b7 d2 f0 ca e8 bb d6 b7 c5 b1 91 c6 af 82 c4 ad 8d dd b1 c4 a3 83 ce a7 c9 a0 82 ae 8c ed 8e fa 93 e5 80 df b2 dd b9 dc fe c4 e6 88 e7 89 ec ce e2 c0 ae cb b3 c7 98 f9 9a ee 87 e8 86 a4 9e e5 c7 b3 ca ba df fd c7 ea db a6 8a a8 c6 b2 d1 8e fd 89 e8 9c f9 db e1 d1 fd df ba c8 ba e5 86 e9 8d e8 ca f0 c0 bd c0 ec ce ab c6 a3 d7 b2 c0 e2 d8 a3 81 e6 83 f7 a8 da bf de b2 c6 af c2 a7 85 bf c4 e6 85 f0 82 f0 95 fb 8f d0 bd dc fe c4 f3 c5 e9 cb bd d2 be ca ab cc a9 f6 9b ed cf f5 c7 f2 c3 f3 c0 f7 db f9 89 e6 91 f4 86 d9 b4 c3 e1 db e2 d4 e3 da f6 d4 a0 cf bb da b6 e9 9e f6 d4 ee dd eb db ea c6 e4 81 f3 81 de bd d2 b6 d3 f1 cb fb 86 fb 86")]
	[InlineData("d0 f2 81 f8 8b ff 9a f7 d5 ef 94 b6 d1 b4 c0 9f ec 95 e6 8f e1 87 e8 ca f0 8b a9 da ad f2 84 e1 93 b1 8b a9 98 b6 86 a8 99 af 8f cd b8 d1 bd d9 f9 cb fa ca f8 c8 fd dd 8f ea 86 a8 99 af 9c ab 98 ad 8f a3 81 e9 9e c1 b7 d2 a0 82 b8 9a ab 85 b5 97 bb 99 f4 9b ff 9a f6 d4 ee cc 87 d7 e6 d7 e2 ca 9f d4 fd df f3 d1 b5 d0 a6 cf ac c9 80 e4 c6 fc de e6 d6 e6 d0 e4 a1 e7 d6 e7 d4 e7 d0 e8 ab 99 df 9a db e9 ab 9e df 9d db e3 d2 eb de 9d ab 9e db ea af 9e ae 99 ad 99 a8 8a a6 84 eb 8e e3 aa ce ec d6 f4 b7 80 c1 f2 c4 81 b1 f2 c0 84 b0 f2 b3 f1 c5 f1 b5 f0 b4 82 c7 81 b1 89 be 8e cf 8c bb 8b bc fa d8 f4 d6 be c9 80 e4 c6 fc de ed d4 91 a9 9d ad 95 d0 94 ad 9a ae ea ae 98 a1 e5 dd 9c ab 9c d8 e1 a7 9f a8 90 a1 97 a4 93 d6 f4 d8 fa 88 fb 88 e1 c3 f9 d4 e2 d0 fc de b2 d3 a7 ce ba cf ab ce 91 f8 da e0 d5 e6 d3 e0 d0 e3 cf ed 81 ee 80 e7 8e fa 8f eb 8e d1 b8 9a a0 8d bf 8e ba 8a b2 9e bc dd b1 d8 b9 ca e8 d2 f0 86 f4 d4 b2 c0 af c1 b5 97 bb 99 ea 9e ff 8b fe 8d af 95 b7 d4 bb d5 b3 da bd c8 ba df bb 99 b5 97 f8 9a fe a1 d2 a0 c3 e1 db f9 8d fd 91 f8 96 fd df f3 d1 bc d5 b6 e9 9d e4 94 f1 d3 e9 cb 82 cd 99 b7 e4 a9 e8 ba ee be f2 a7 e0 b3 e4 ad f9 ba f2 d0 fc de b8 dd bc c8 bd cf aa 88 b2 90 c4 8d c0 fa bf f1 b4 96 ba 98 f5 94 f7 d5 ef cd ff c7 fd b8 fd c7 f2 c0 fa bf fd c7 f7 b6 8c cd f9 db f7 d5 a0 d0 b4 d5 a1 c8 a6 c1 e3 d9 e9 c5 e7 8b ee 8a d5 ba dc ba 98 a2 92 be 9c ee 8b e7 86 ff a0 d3 a7 c6 b2 d7 f5 cf ff d3 f1 9e f0 af db b2 df ba 98 a2 92 be 9c f5 96 f9 97 c8 a0 c1 b2 da f8 c2 e0 c2 ee cc a8 cd bb e4 8a eb 86 e3 c1 fb d9 8a e7 86 f4 80 a0 f7 9e b3 f5 9c bc ec 80 f5 92 b2 ff 96 f8 91 b3 9f bd dc bf cb a2 d4 b1 ee 83 ec 88 ed cf f5 d7 b9 d6 b8 dd ff d3 f1 9f fa 82 f6 a9 c8 ab df b6 d9 b7 95 af d4 f6 82 fb 8b ee cc f6 db ea 97 bb 99 f7 83 e0 bf cc b8 d9 ad c8 ea d0 e0 cc ee 8b f9 8b d4 b7 d8 bc d9 fb c1 f1 8c f1 dd ff 9a f7 92 e6 83 f1 d3 e9 92 b0 d7 b2 c6 99 eb 8e ef 83 f7 9e f3 96 b4 8e f5 d7 b4 c1 b3 c1 a4 ca be e1 8c ed cf f5 c5 e9 cb bd d2 be ca ab cc a9 f6 9b ed cf f5 c7 f3 c6 f0 c5 f5 d9 fb 8b e4 93 f6 84 db b6 c1 e3 d9 e9 c5 e7 93 fc 88 e9 85 da ad c5 e7 dd ef c3 e1 84 f6 84 db b8 d7 b3 d6 f4 ce fe 83 fe 83")]
	public void DecryptDecodeTests(string input)
	{
		byte[] bytes = (
			from s in input.Split(' ')
			select Convert.ToByte(s, fromBase: 16)
			).ToArray();

		var json = bytes.Decrypt().Decode();
	}

	[Theory]
	[InlineData(
		@"{""current_ma"":76,""voltage_mv"":251037,""power_mw"":9679,""total_wh"":3601,""err_code"":0}",
		76, 251_037, 9_679)]
	public void DeserializeEmeterTests(string json, int expectedMilliAmps, int expectedMilliVolts, int expectedMilliWatts)
	{
		var actual = JsonSerializer.Deserialize<Models.RealtimeInfoObject>(json);
		Assert.Equal(expectedMilliAmps, actual.current_ma);
		Assert.Equal(expectedMilliVolts, actual.voltage_mv);
		Assert.Equal(expectedMilliWatts, actual.power_mw);

		var (actualMilliAmps, actualMilliVolts, actualMilliWats) = actual;
		Assert.Equal(expectedMilliAmps, actualMilliAmps);
		Assert.Equal(expectedMilliVolts, actualMilliVolts);
		Assert.Equal(expectedMilliWatts, actualMilliWats);
	}
}
