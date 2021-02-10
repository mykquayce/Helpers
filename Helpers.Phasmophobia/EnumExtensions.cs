using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Helpers.Phasmophobia
{
	public static class EnumExtensions
	{
		private readonly static IFormatProvider _formatProvider = CultureInfo.InvariantCulture;

		public static IEnumerable<T> GetFlags<T>(this T @enum) where T : IConvertible
		{
			var left = @enum.ToInt64(_formatProvider);

			foreach (T item in Enum.GetValues(typeof(T)))
			{
				var right = item.ToInt64(_formatProvider);

				if (left == right
					|| (left & right) != 0)
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<Evidences> GetEvidences(this Ghost ghost) => ((Evidences)ghost).GetFlags();

		public static IEnumerable<Ghost> GetGhosts(this Evidences evidences)
		{
			foreach (Ghost ghost in Enum.GetValues(typeof(Ghost)))
			{
				if (((Evidences)ghost & evidences) == evidences)
				{
					yield return ghost;
				}
			}
		}

		public static IEnumerable<Ghost> GetEliminatedGhosts(this Evidences evidences)
		{
			foreach (Ghost ghost in Enum.GetValues(typeof(Ghost)))
			{
				if (((Evidences)ghost & evidences) == 0)
				{
					yield return ghost;
				}
			}
		}

		public static IEnumerable<Ghost> GetGhosts(this Evidences found, Evidences eliminated)
		{
			var left = found.GetGhosts();
			var right = eliminated.GetEliminatedGhosts();

			return from l in left
				   from r in right
				   where l == r
				   select l;
		}
	}
}
