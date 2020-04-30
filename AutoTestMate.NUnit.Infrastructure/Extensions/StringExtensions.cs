namespace AutoTestMate.NUnit.Infrastructure.Extensions
{
	public static class StringExtensions
	{
        /// <summary>
		/// Returns null if the string is null or white space. Otherwise
		/// returns the string that was passed in.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>The string that was passed in, or null if the string was null or white space.</returns>
		public static string ToNullWhenWhiteSpace(this string s)
		{
			return string.IsNullOrWhiteSpace(s) ? null : s;
		}
    }
}