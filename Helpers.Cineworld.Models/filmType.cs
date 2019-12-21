using Helpers.Cineworld.Models.Enums;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Helpers.Cineworld.Models
{
	public partial class filmType
	{
		private const RegexOptions _regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
		private static readonly Regex _lengthRegex = new Regex(@"^(\d{2,3}) mins$", _regexOptions);

		public filmType()
		{
			this.PropertyChanged += FilmType_PropertyChanged;
		}

		public int Duration { get; set; }

		[field: NonSerialized]
		public Formats Formats { get; set; }

		[field: NonSerialized]
		public string? FixedTitle { get; set; }

		private void FilmType_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is filmType film)) return;

			switch (e.PropertyName)
			{
				case nameof(length):
					var s = _lengthRegex.Replace(film.length, "$1");
					Duration = !string.IsNullOrWhiteSpace(s) ? int.Parse(s) : default;
					break;
				case nameof(title):
					string @fixed;
					(@fixed, Formats) = film.title.DeconstructTitle();
					FixedTitle = @fixed.DeArticlize();
					break;
			}
		}
	}
}
