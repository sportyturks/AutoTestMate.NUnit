using System.Diagnostics.CodeAnalysis;
using AutoTestMate.NUnit.Infrastructure.Core;
using OpenQA.Selenium;

namespace AutoTestMate.NUnit.Web.Core
{
    [ExcludeFromCodeCoverage]
    public abstract class BasePage
    {
        private readonly IWebDriver _driver;
        private readonly IConfigurationReader _configurationReader;
        private readonly ILoggingUtility _loggingUtility;

        /// <summary>
        /// Constructor of Base Page
        /// </summary>
        protected BasePage(IWebDriver driver, IConfigurationReader configurationReader, ILoggingUtility loggingUtility)
        {
            _driver = driver;
            _configurationReader = configurationReader;
            _loggingUtility = loggingUtility;
        }

        public IWebDriver Driver => _driver;

        public IConfigurationReader ConfigurationReader => _configurationReader;

        public ILoggingUtility LoggingUtility => _loggingUtility;
    }
}