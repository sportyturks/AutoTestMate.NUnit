namespace AutoTestMate.NUnit.Web.Core.Browser
{
	public class BrowserTypeMapper
	{
		public static Enums.BrowserTypes ConvertBrowserValue(string browserType)
        {
            var browserTypeValue = browserType.ToLower();
            return browserTypeValue switch
            {
                Constants.BrowserType.Chrome => Enums.BrowserTypes.Chrome,
                Constants.BrowserType.InternetExplorer => Enums.BrowserTypes.InternetExplorer,
                Constants.BrowserType.Firefox => Enums.BrowserTypes.Firefox,
                Constants.BrowserType.Edge => Enums.BrowserTypes.Edge,
                _ => Enums.BrowserTypes.InternetExplorer
            };
        }
	}
}
