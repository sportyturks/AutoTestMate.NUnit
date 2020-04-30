using System;
using AutoTestMate.NUnit.Infrastructure.Constants;
using AutoTestMate.NUnit.Infrastructure.Core;
using AutoTestMate.NUnit.Web.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace AutoTestMate.NUnit.Web.Core.Browser
{
    public class BrowserOptionsFactory : IFactory<DriverOptions>
    {
        protected readonly IConfigurationReader _configurationReader;
        protected readonly ILoggingUtility _loggingUtility;
        public BrowserOptionsFactory(IConfigurationReader configurationReader, ILoggingUtility loggingUtility)
        {
            _configurationReader = configurationReader;
            _loggingUtility = loggingUtility;
        }
        public virtual DriverOptions Create()
        {
            var browserTypeValue = _configurationReader.GetConfigurationValue(Constants.Configuration.BrowserTypeKey);
            var browserType = !string.IsNullOrWhiteSpace(browserTypeValue) ? BrowserTypeMapper.ConvertBrowserValue(browserTypeValue) : BrowserTypes.InternetExplorer;

            switch (browserType)
            {
                case BrowserTypes.Firefox:
                    return CreateFirefoxDriverOptions();
                case BrowserTypes.InternetExplorer:
                    return CreateInternetExplorerDriverOptions();
                case BrowserTypes.Chrome:
                    return CreateChromeDriverOptions();
                case BrowserTypes.Edge:
                    return CreateEdgeDriverOptions();
                case BrowserTypes.NotSet:
                    return CreateInternetExplorerDriverOptions();                    
                default:
                    return CreateInternetExplorerDriverOptions();
            }
        }

	    protected virtual DriverOptions CreateFirefoxDriverOptions()
        {
            try
            {
                var options = new FirefoxOptions();
                var profileManager = new FirefoxProfileManager();
                var browserProfileSetting = _configurationReader.GetConfigurationValue(Constants.Configuration.BrowserProfileKey);
                var enableDetailLoggingSetting = _configurationReader.GetConfigurationValue(Constants.Configuration.EnableDetailedLogging).ToLower();
                var headlessSetting = _configurationReader.GetConfigurationValue(Constants.Configuration.HeadlessKey).ToLower();
                _loggingUtility.Info($"Browser Profile: {browserProfileSetting}, Detailed Logging: {enableDetailLoggingSetting}, Headless: {headlessSetting}");
                var profile = profileManager.GetProfile(string.IsNullOrWhiteSpace(browserProfileSetting) ? "default" : browserProfileSetting);
                profile.AcceptUntrustedCertificates = true;
                profile.AssumeUntrustedCertificateIssuer = true;
                options.Profile = profile;
                options.AddArguments("--width-1920");
                options.AddArguments("--height-1080");

                if (string.Equals(enableDetailLoggingSetting, Generic.TrueValue))
                {
                    options.LogLevel = FirefoxDriverLogLevel.Trace;
                }

                if (string.Equals(headlessSetting, Generic.TrueValue))
                {
                    options.AddArguments("--headless");
                    /*options.AddArgument("disable-extensions");
                    options.AddArgument("disable-gpu");
                    options.AddArgument("disable-infobars");*/
                }

                return options;

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                _loggingUtility.Error($"Exeception: {e.Message}, Inner Exception: {e.InnerException?.Message}");
                throw;
            }
        }

	    protected virtual DriverOptions CreateChromeDriverOptions()
        {
            var options = new ChromeOptions();

            options.LeaveBrowserRunning = true;
			options.AddArgument("bwsi");
			options.AddArgument("ignore-certificate-errors");
			//options.AddArgument("disable-extensions");
			options.AddArgument("window-size=1920,1080");
	        options.AddArgument("start-maximized");
			options.AddArgument("allow-insecure-localhost");
            options.AddArgument("no-sandbox");
            options.AddAdditionalCapability("useAutomationExtension", false);


	        if (string.Equals(_configurationReader.GetConfigurationValue(Constants.Configuration.EnableDetailedLogging).ToLower(), Generic.TrueValue))
	        {
		        var perfLogPrefs = new ChromePerformanceLoggingPreferences();
		        perfLogPrefs.AddTracingCategories("devtools.network");
		        options.PerformanceLoggingPreferences = perfLogPrefs;
		        options.AddAdditionalCapability(CapabilityType.EnableProfiling, true, true);
		        options.SetLoggingPreference("performance", LogLevel.All);
	        }

	        if (string.Equals(_configurationReader.GetConfigurationValue(Constants.Configuration.HeadlessKey).ToLower(), Generic.TrueValue))
            {
                options.AddArguments("headless");
                options.AddArgument("verbose");
                options.AddArgument("disable-gpu");
                options.AddArgument("allow-running-insecure-content");
	            options.AddAdditionalCapability(CapabilityType.AcceptSslCertificates, true, true);
	            options.AddAdditionalCapability(CapabilityType.AcceptInsecureCertificates, true, true);                
                /*options.AddArgument("disable-extensions");
                options.AddArgument("disable-gpu");
                options.AddArgument("disable-infobars");*/
            }

			return options;
        }

	    protected virtual DriverOptions CreateInternetExplorerDriverOptions()
        {
            var options =
                new InternetExplorerOptions
                {
                    ElementScrollBehavior = InternetExplorerElementScrollBehavior.Top,
                    EnableNativeEvents = true,
                    EnsureCleanSession = true,
                    IgnoreZoomLevel = true,
                    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                    RequireWindowFocus = false,
                    EnablePersistentHover = false,
                    PageLoadStrategy =  PageLoadStrategy.Normal
                };

            return options;
        }
		protected virtual DriverOptions CreateEdgeDriverOptions()
        {
            var options = new EdgeOptions();
            options.AddAdditionalCapability(CapabilityType.AcceptSslCertificates, true);
            options.AddAdditionalCapability(CapabilityType.IsJavaScriptEnabled, true);
            options.AddAdditionalCapability(CapabilityType.AcceptInsecureCertificates, true);
            options.AddAdditionalCapability(CapabilityType.SupportsFindingByCss, true);
            return options;
        }
    }
}