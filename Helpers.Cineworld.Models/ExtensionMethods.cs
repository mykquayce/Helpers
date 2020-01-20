using Helpers.Cineworld.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Helpers.Cineworld.Models
{
	public static class ExtensionMethods
	{
		public static (string, Formats) DeconstructTitle(this string title)
		{
			if (string.IsNullOrWhiteSpace(title)) return (title, Formats.None);

			Formats formats = Formats.None;

			if (title.EndsWith(" : Unlimited Screening"))
			{
				title = title[..^22];
				formats |= Formats.SecretUnlimitedScreening;
			}
			else if (title.EndsWith(": Unlimited Screening"))
			{
				title = title[..^21];
				formats |= Formats.SecretUnlimitedScreening;
			}
			else if (title.EndsWith(" Unlimited Screening"))
			{
				title = title[..^20];
				formats |= Formats.SecretUnlimitedScreening;
			}

			if (title.StartsWith("(2D) "))
			{
				return (title[5..], formats | Formats._2d);
			}

			if (title.StartsWith("(3D) "))
			{
				return (title[5..], formats | Formats._3d);
			}

			if (title.StartsWith("(4DX) "))
			{
				return (title[6..], formats | Formats._2d | Formats._4dx);
			}

			if (title.StartsWith("(IMAX) "))
			{
				return (title[7..], formats | Formats._2d | Formats.Imax);
			}

			if (title.StartsWith("(IMAX 3-D) "))
			{
				return (title[11..], formats | Formats._3d | Formats.Imax);
			}

			if (title.StartsWith("(ScreenX) "))
			{
				return (title[10..], formats | Formats._2d | Formats.ScreenX);
			}

			if (title.StartsWith("(SS) "))
			{
				return (title[5..], formats | Formats._2d | Formats.Subtitled);
			}

			if (title.StartsWith("Autism Friendly Screening: "))
			{
				return (title[27..], formats | Formats._2d | Formats.AutismFriendlyScreening);
			}

			if (title.StartsWith("M4J "))
			{
				return (title[4..], formats | Formats._2d | Formats.MoviesForJuniors);
			}

			if (title.StartsWith("SubM4J "))
			{
				return (title[7..], formats | Formats._2d | Formats.MoviesForJuniors | Formats.Subtitled);
			}

			return (title, formats | Formats._2d);
		}

		public static string? DeArticlize(this string? title)
		{
			var words = title?.Split(' ');

			switch (words?.Length ?? 0)
			{
				case 0:
				case 1:
					return title;
				default:
					return title!.Split(' ')[0] switch
					{
						"A" => title[2..] + ", A",
						"An" => title[3..] + ", An",
						"The" => title[4..] + ", The",
						_ => title,
					};
			}
		}

		public static IEnumerable<byte> ToHours(this TimesOfDay timesOfDay)
		{
			if ((timesOfDay & TimesOfDay.Night) != 0)
			{
				yield return 0;
				yield return 1;
				yield return 2;
				yield return 3;
				yield return 4;
				yield return 5;
			}

			if ((timesOfDay & TimesOfDay.Morning) != 0)
			{
				yield return 6;
				yield return 7;
				yield return 8;
				yield return 9;
				yield return 10;
				yield return 11;
			}

			if ((timesOfDay & TimesOfDay.Afternoon) != 0)
			{
				yield return 12;
				yield return 13;
				yield return 14;
				yield return 15;
				yield return 16;
				yield return 17;
			}

			if ((timesOfDay & TimesOfDay.Evening) != 0)
			{
				yield return 18;
				yield return 19;
				yield return 20;
				yield return 21;
				yield return 22;
				yield return 23;
			}
		}

		public static TimesOfDay ToTimesOfDay(this IEnumerable<DateTime> dateTimes)
		{
			var results = TimesOfDay.None;

			foreach (var dateTime in dateTimes)
			{
				var hour = dateTime.Hour;

				if (hour < 6)
				{
					results |= TimesOfDay.Night;
				}
				else if (hour < 12)
				{
					results |= TimesOfDay.Morning;
				}
				else if (hour < 18)
				{
					results |= TimesOfDay.Afternoon;
				}
				else
				{
					results |= TimesOfDay.Evening;
				}
			}

			return results;
		}

		public static short ParseLength(this string s)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				throw new ArgumentNullException(nameof(s));
			}

			if (short.TryParse(s[..^5], out var result))
			{
				return result;
			}

			throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected value for {nameof(s)}: {s}")
			{
				Data = { [nameof(s)] = s, },
			};
		}
	}
}
