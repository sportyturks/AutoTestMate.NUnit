using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using AutoTestMate.NUnit.Infrastructure.Constants;
using AutoTestMate.NUnit.Infrastructure.Core;
using AutoTestMate.NUnit.Infrastructure.Helpers;
using AutoTestMate.NUnit.Web.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Configuration = AutoTestMate.NUnit.Web.Constants.Configuration;

namespace AutoTestMate.NUnit.Web.Core.Browser
{
    [ExcludeFromCodeCoverage]
    public class BrowserFactory : IFactory<IWebDriver>
    {
        protected readonly IFactory<DriverOptions> BrowserOptionsFactory;
		public IConfigurationReader ConfigurationReader { get; set; }

        /// <summary>
        /// Used to create drivers based on the passed in runnsettings, appsetting or datarow stored in the configuration reader
        /// </summary>
        /// <param name="configurationReader">settings to be consumed while creating the driver</param>
        /// <param name="browserOptionsFactory"></param>
        public BrowserFactory(IConfigurationReader configurationReader, IFactory<DriverOptions> browserOptionsFactory)
        {
            ConfigurationReader = configurationReader;
            BrowserOptionsFactory = browserOptionsFactory;
        }

        public virtual IWebDriver Create()
        {
            IWebDriver driver;
            var loginWaitTime = Convert.ToInt64(ConfigurationReader.GetConfigurationValue(Constants.Configuration.LoginWaitTimeKey));

            var browserTypeValue = ConfigurationReader.GetConfigurationValue(Constants.Configuration.BrowserTypeKey);
            var browserType = !string.IsNullOrWhiteSpace(browserTypeValue) ? BrowserTypeMapper.ConvertBrowserValue(browserTypeValue) : BrowserTypes.InternetExplorer;

            switch (browserType)
            {
                case BrowserTypes.Firefox:
                    driver = CreateFirefoxWebDriver(loginWaitTime);
	                break;
                case BrowserTypes.InternetExplorer:
                    driver = CreateInternetExplorerWebDriver(loginWaitTime);
	                break;
                case BrowserTypes.Chrome:
					driver = CreateChromeWebDriver(loginWaitTime);
	                break;
                case BrowserTypes.Edge:
                    driver = CreateEdgeWebDriver(loginWaitTime);
	                break;
                case BrowserTypes.NotSet:
	                driver = CreateInternetExplorerWebDriver(loginWaitTime);
					break;
                default:
	                driver = CreateInternetExplorerWebDriver(loginWaitTime);
					break;
            }

            return driver;
        }

	    protected virtual IWebDriver CreateEdgeWebDriver(long loginWaitTime)
	    {
		    DriverOptions driverOptions;
		    IWebDriver driver;
		    driverOptions = BrowserOptionsFactory.Create();
		    driver = new EdgeDriver(ConfigurationReader.GetConfigurationValue(Configuration.DriverServerLocationKey), (EdgeOptions) driverOptions, TimeSpan.FromMinutes(loginWaitTime));
		    driver.Manage().Window.Maximize();
		    return driver;
	    }

	    protected virtual IWebDriver CreateChromeWebDriver(long loginWaitTime)
	    {
		    DriverOptions driverOptions;
		    IWebDriver driver;
		    driverOptions = BrowserOptionsFactory.Create();

		    if (string.Equals(ConfigurationReader.GetConfigurationValue(Configuration.EnableDetailedLogging).ToLower(), Generic.TrueValue))
		    {
			    var service = ChromeDriverService.CreateDefaultService(ConfigurationReader.GetConfigurationValue(Configuration.DriverServerLocationKey));
			    service.LogPath = "chromedriver.log";
			    service.EnableVerboseLogging = true;
			    driver = new ChromeDriver(service, (ChromeOptions) driverOptions, TimeSpan.FromMinutes(loginWaitTime));
		    }
		    else
		    {
				var driverServerLocation = string.IsNullOrWhiteSpace(ConfigurationReader.GetConfigurationValue(Configuration.DriverServerLocationKey)) ? FileHelper.GetCurrentExecutingDirectory() : ConfigurationReader.GetConfigurationValue(Constants.Configuration.DriverServerLocationKey);
				driver = new ChromeDriver(driverServerLocation, (ChromeOptions) driverOptions, TimeSpan.FromMinutes(loginWaitTime));
		    }

		    driver.Manage().Cookies.DeleteAllCookies();
		    return driver;
	    }

	    protected virtual IWebDriver CreateInternetExplorerWebDriver(long loginWaitTime)
	    {
		    DriverOptions driverOptions;
		    IWebDriver driver;
		    driverOptions = BrowserOptionsFactory.Create();
		    driver = new InternetExplorerDriver(ConfigurationReader.GetConfigurationValue(Configuration.DriverServerLocationKey), (InternetExplorerOptions) driverOptions, TimeSpan.FromMinutes(loginWaitTime));
		    driver.Manage().Cookies.DeleteAllCookies();
		    driver.Manage().Window.Maximize();
		    return driver;
	    }

	    protected virtual IWebDriver CreateFirefoxWebDriver(long loginWaitTime)
	    {
		    DriverOptions driverOptions;
		    IWebDriver driver;
		    driverOptions = BrowserOptionsFactory.Create();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var ffService = FirefoxDriverService.CreateDefaultService(ConfigurationReader.GetConfigurationValue(Configuration.DriverServerLocationKey), "geckodriver.exe");
		    ffService.FirefoxBinaryPath = string.IsNullOrWhiteSpace(ConfigurationReader.GetConfigurationValue(Configuration.DriverServerLocationKey)) ? @"C:\Program Files\Mozilla Firefox\firefox.exe" : ConfigurationReader.GetConfigurationValue(Configuration.BrowserLocationKey);
		    driver = new FirefoxDriver(ffService, (FirefoxOptions) driverOptions, TimeSpan.FromMinutes(loginWaitTime));
		    driver.Manage().Cookies.DeleteAllCookies();
		    driver.Manage().Window.Maximize();
		    return driver;
	    }


	    public string AssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
