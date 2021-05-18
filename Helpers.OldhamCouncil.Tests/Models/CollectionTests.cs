using System.IO;
using System.Text;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.OldhamCouncil.Tests.Models
{
	public class CollectionTests
	{
		private readonly Encoding _encoding = Encoding.UTF8;
		private readonly XmlSerializerFactory _xmlSerializerFactory = new();

		public T Deserialize<T>(string xml)
		{
			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
			var bytes = _encoding.GetBytes(xml);
			using var stream = new MemoryStream(bytes);
			return (T)serializer.Deserialize(stream)!;
		}

		[Theory]
		[InlineData(@"<table id=""xform_confirmation_table"" class=""data-table confirmation"">
	<thead>
		<tr>
			<th style=""width:50%;""><b>Brown Bin</b></th>
			<th></th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>
				<b>Collection Date</b>
			</td>
			<td class=""coltwo"">
				05/05/2021
			</td>
		</tr>
		<tr>
			<td>
				<b>Collection Day</b>
			</td>
			<td class=""coltwo"">
				Wednesday
			</td>
		</tr>
	</tbody>
</table>", "Brown Bin", "05/05/2021")]
		public void DeserializeBinsHtml(string html, string expectedBin, string expectedDate)
		{
			var table = Deserialize<OldhamCouncil.Models.Generated.tableType>(html);

			Assert.Equal(expectedBin, table.thead.tr.th[0].b);
			Assert.Equal(expectedDate, table.tbody[0].td[1].Text[0].Trim());
		}
	}
}
