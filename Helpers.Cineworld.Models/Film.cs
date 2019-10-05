using System;
using System.Text.RegularExpressions;

namespace Helpers.Cineworld.Models
{
	public partial class filmType
	{

		private const RegexOptions _regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
		private static readonly Regex _lengthRegex = new Regex(@"^(\d{2,3}) mins$", _regexOptions);

		public TimeSpan Duration => TimeSpan.FromMinutes(int.Parse(_lengthRegex.Replace(length, "$1")));
	}
}
