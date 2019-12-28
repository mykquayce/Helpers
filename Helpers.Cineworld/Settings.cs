using System;

namespace Helpers.Cineworld
{
	internal static class Settings
	{
		public static Uri BaseAddress { get; } = new Uri("https://www.cineworld.co.uk/", UriKind.Absolute);
		public static Uri AllPerformancesPath { get; } = new Uri("/syndication/all-performances.xml", UriKind.Relative);
		public static Uri ListingsPath { get; } = new Uri("/syndication/listings.xml", UriKind.Relative);
		public static string LastModifiedHeaderKey { get; } = "Last-Modified";
	}
}
