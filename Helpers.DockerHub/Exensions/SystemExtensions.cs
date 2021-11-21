namespace System;

public static class SystemExtensions
{
	public static byte[] ToUtf8Bytes(this string s) => System.Text.Encoding.UTF8.GetBytes(s);
	public static string ToBase64String(this byte[] bytes) => Convert.ToBase64String(bytes, Base64FormattingOptions.None);
	public static string ToBase64String(this string s) => s.ToUtf8Bytes().ToBase64String();
}
