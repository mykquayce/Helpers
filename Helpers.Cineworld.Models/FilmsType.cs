using System.Xml.Schema;
using System.Xml.Serialization;

namespace Helpers.Cineworld.Models
{
	[XmlRoot("Films", Namespace = "", IsNullable = false)]
	public partial class FilmsType
	{
		[XmlElement("Film", Form = XmlSchemaForm.Unqualified)]
		public Generated.FilmType[]? Film;
	}
}
