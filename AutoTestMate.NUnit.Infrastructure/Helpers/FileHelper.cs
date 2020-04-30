using System;
using System.IO;
using System.Reflection;
using AutoTestMate.NUnit.Infrastructure.Extensions;

namespace AutoTestMate.NUnit.Infrastructure.Helpers
{
	public class FileHelper
	{
		#region Get Current Executing Directory

		public static string GetCurrentExecutingDirectory()
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

			return directoryName?.Replace("file:\\", string.Empty).Replace(@"file:\", string.Empty) ?? string.Empty;
		}

		#endregion

		#region File Contents to String (obsolete)

		[Obsolete("Use the System.IO method File.ReadAllText instead.")]
		public static string FileContentsToString(string file)
		{
			return File.ReadAllText(file);
		}

		#endregion

		#region Remove/Replace Invalid File Name Characters

		/// <summary>
		/// Removes any characters that cannot be used as part of a file name from <paramref name="name"/>.
		/// If the result is an empty string, or a string containing only white space, will return null.
		/// </summary>
		/// <param name="name">The string from which to remove invalid file name characters.</param>
		/// <returns>A string containing the remaining characters, or null if only white space remains.</returns>
		public static string RemoveInvalidFileNameChars(string name)
		{
			return string.Concat(name.Split(Path.GetInvalidFileNameChars())).ToNullWhenWhiteSpace();
		}

		/// <summary>
		/// Replaces any characters that cannot be used as part of a file name from <paramref name="name"/>, and
		/// replaces them with <paramref name="replace"/>. It is up to the calling method to ensure that
		/// <paramref name="replace"/> does not contain any invalid characters.
		/// </summary>
		/// <param name="name">The string from which to remove invalid file name characters.</param>
		/// <param name="replace">The string with which to replace any invalid file name characters.</param>
		/// <returns>A string containing the remaining characters, or null if only white space remains.</returns>
		public static string ReplaceInvalidFileNameChars(string name, string replace)
		{
			return string.Join(replace, name.Split(Path.GetInvalidFileNameChars())).ToNullWhenWhiteSpace();
		}

		#endregion
	}
}