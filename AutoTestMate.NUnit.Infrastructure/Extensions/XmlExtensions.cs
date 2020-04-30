using AutoTestMate.MsTest.Infrastructure.Helpers;

namespace AutoTestMate.MsTest.Infrastructure.Extensions
{
	/// <summary>
	/// A collection of extension methods related to the creation and modification of XML.
	/// </summary>
	public static class XmlExtensions
	{
		#region Serialize Object to XML

		/// <summary>
		/// Serialises an object that has been marked with the DataContract attribute to an XML string.
		/// </summary>
		/// <param name="o">The object to serialise.</param>
		/// <returns></returns>
		public static string ToXml(this object o)
		{
			return XmlHelper.Serialise(o);
		}

		#endregion
	}
}
