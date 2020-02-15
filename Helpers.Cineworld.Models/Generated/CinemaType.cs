using System.Text.RegularExpressions;

namespace Helpers.Cineworld.Models.Generated
{
	public partial class CinemaType
	{
		private const RegexOptions _regexOptions = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;
		private static readonly Regex _nameRegex = new Regex(@"^Cineworld (?:- )?(?<Name>.+?)$", _regexOptions);

		public CinemaType()
		{
			PropertyChanged += CinemaType_PropertyChanged;
		}

		private void CinemaType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!(sender is CinemaType cinemaType))
			{
				return;
			}

			switch (e.PropertyName)
			{
				case nameof(Name):

					var match = _nameRegex.Match(cinemaType.Name);

					if (match.Success)
					{
						cinemaType.Name = match.Groups["Name"].Value;
					}

					break;
			}
		}
	}
}
