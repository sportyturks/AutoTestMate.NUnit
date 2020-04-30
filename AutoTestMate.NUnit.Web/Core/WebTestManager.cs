using System;
using AutoTestMate.NUnit.Infrastructure.Core;
using AutoTestMate.NUnit.Services.Core;
using AutoTestMate.NUnit.Web.Constants;
using AutoTestMate.NUnit.Web.Core.Browser;
using Castle.MicroKernel.Registration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutoTestMate.NUnit.Web.Core
{
	public class WebTestManager : ServiceTestManager
	{
		#region Private Variables

		private static WebTestManager _uniqueInstance;
		private static readonly object SyncLock = new Object();

		#endregion

		#region Properties

		public new static WebTestManager Instance()
		{
			// Lock entire body of method
			lock (SyncLock)
			{
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_uniqueInstance == null)
				{
					_uniqueInstance = new WebTestManager();
				}
				return _uniqueInstance;
			}
		}

    	public bool IsDriverNull => !Container.Kernel.HasComponent(typeof(IWebDriver)) || Container.Resolve<IWebDriver>() == null;

		public WebDriverWait BrowserWait
		{
			get
			{
                if (IsDriverNull)
                {
                    throw new NullReferenceException(Exceptions.ExceptionMsgWebBrowserWaitInstanceNotInitialised);
                }

				return Container.Resolve<WebDriverWait>();
			}
			set => Container.Register(Component.For<WebDriverWait>().Instance(value).OverridesExistingRegistration().LifestyleSingleton());
		}

		public IWebDriver Browser
		{
			get
			{
				if (IsDriverNull)
                {
                    throw new NullReferenceException(Exceptions.ExceptionMsgWebBrowserWaitInstanceNotInitialised);
				}

				return  Container.Resolve<IWebDriver>();
			}
			private set => Container.Register(Component.For<IWebDriver>().Instance(value).OverridesExistingRegistration().LifestyleSingleton());
		}

		#endregion

		#region Constructor

		private WebTestManager() { }

		#endregion

		#region Public Methods

        public override void OnInitialiseAssemblyDependencies(TestParameters testParameters = null)
        {
            base.OnInitialiseAssemblyDependencies(testParameters);

            var browserOs = ConfigurationReader.GetConfigurationValue(Configuration.BrowserOsKey).ToLower().Trim();
            if (string.Equals(browserOs, Configuration.BrowserOsLinux.ToLower()))
            {
                Container.Register(Component.For<IProcess>().ImplementedBy<LinuxOsProcess>().LifestyleSingleton());
            }
            else
            {
                Container.Register(Component.For<IProcess>().ImplementedBy<WinOsProcess>().LifestyleSingleton());
            }
		}

        public override void InitialiseIoc()
		{
			base.InitialiseIoc();
            
			Container.Register(Component.For<IFactory<IDriverCleanup>>().ImplementedBy<BrowserCleanupFactory>().LifestyleSingleton())
                .Register(Component.For<IFactory<DriverOptions>>().ImplementedBy<BrowserOptionsFactory>().LifestyleSingleton())
				.Register(Component.For<IFactory<IWebDriver>>().ImplementedBy<BrowserFactory>().LifestyleSingleton());
        }

        public override void OnTestMethodInitialise(TestParameters testParameters = null)
		{
			if (IsInitialised) throw new ApplicationException(Exceptions.ExceptionMsgSingletonAlreadyInitialised);

			try
			{
				InitialiseTestContext(testParameters);
				StartWebDriver();				
			}
			catch (System.Exception exp)
			{
                LoggingUtility?.Error($"Exception Msg: {exp.Message}, Exception StackTrace: {exp.StackTrace}, Inner Exception Msg: {exp.InnerException?.Message} Inner Exception Stack Trace: {exp.InnerException?.StackTrace}");
				Dispose();
				throw;
			}
		}

		public override void Dispose()
		{
			DisposeInternal();
		}

		#endregion

		#region Private Methods

		public override void DisposeInternal()
		{
			try
			{
				if (IsInitialised && !IsDriverNull)
				{
					Browser?.Quit();
				}
			}
			catch (System.Exception exp)
			{
				LoggingUtility.Error(exp.Message);
			}
			finally
			{
				if (IsInitialised)
				{
					Browser?.Dispose();
					var driverCleanup = Container.Resolve<IDriverCleanup>();
					driverCleanup?.Dispose();
				}

				IsInitialised = false;
			}
		}
		public void StartWebDriver()
		{
			var browserDriverCleanupFactory = Container.Resolve<IFactory<IDriverCleanup>>();
			var browserFactory = Container.Resolve<IFactory<IWebDriver>>();

			var driverCleanup = browserDriverCleanupFactory.Create();
			driverCleanup.Initialise();
			Container.Register(Component.For<IDriverCleanup>().Instance(driverCleanup).OverridesExistingRegistration().LifestyleSingleton());

			Browser = browserFactory.Create();
			//Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(string.IsNullOrWhiteSpace(UiConfigurationReader.LoginWaitTime) ? 1 : Convert.ToInt32(UiConfigurationReader.LoginWaitTime));

			var browserWait = new WebDriverWait(Browser, TimeSpan.FromMinutes(string.IsNullOrWhiteSpace(ConfigurationReader.GetConfigurationValue(Configuration.LoginWaitTimeKey)) ? 1 : Convert.ToInt32(ConfigurationReader.GetConfigurationValue(Configuration.LoginWaitTimeKey))));
			BrowserWait = browserWait;

			IsInitialised = true;
		}

		#endregion
	}
}
