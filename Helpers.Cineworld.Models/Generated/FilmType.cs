using System;

namespace Helpers.Cineworld.Models.Generated
{
	public partial class FilmType
	{
		public FilmType()
		{
			PropertyChanged += FilmType_PropertyChanged;
		}

		public Enums.Formats Formats { get; set; }

		private void FilmType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!(sender is FilmType filmType))
			{
				return;
			}

			switch (e.PropertyName)
			{
				case nameof(Title):

					if (filmType.Formats != Enums.Formats.None)
					{
						break;
					}

					var (newTitle, formats) = filmType.Title.DeconstructTitle();

					newTitle = newTitle.DeArticlize()!;

					filmType.Formats = formats;

					if (string.Equals(filmType.Title, newTitle, StringComparison.InvariantCultureIgnoreCase))
					{
						break;
					}

					filmType.Title = newTitle;

					break;
			}
		}

		public override string ToString()
		{
			var formats = Formats & ~Enums.Formats._2d;

			if (formats == Enums.Formats.None)
			{
				return this.Title;
			}

			return $"{Title} ({formats:G})";
		}
	}
}
