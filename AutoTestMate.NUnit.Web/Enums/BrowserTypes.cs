using System.ComponentModel;

namespace AutoTestMate.NUnit.Web.Enums
{
	public enum BrowserTypes
	{
		[Description("firefox")]
		Firefox,
		[Description("iexplore")]
		InternetExplorer,
		[Description("chrome")]
		Chrome,
		[Description("microsoftwebdriver")]
		Edge,
		[Description("")]
		NotSet
	}
}
