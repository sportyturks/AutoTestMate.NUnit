using System.Net.Http;
using AutoTestMate.NUnit.Infrastructure.Constants;
using AutoTestMate.NUnit.Services.Core;
using AutoTestMate.NUnit.Web.Extensions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace AutoTestMate.NUnit.Web.Core
{
	public abstract class WebTestBase : ServiceTestBase
	{
		[SetUp]
		public override void OnTestInitialise()
		{
			try
			{
				TestManager = WebTestManager.Instance();
				TestManager.OnTestMethodInitialise(TestParameters);
            }
			catch (System.Exception ex)
			{
				if (LoggingUtility == null || ConfigurationReader == null) throw;

				LoggingUtility.Error(Constants.Exceptions.ExceptionMsgSetupError + ex.Message);

                throw;
            }
		}

		[TearDown]
		public override void OnTestCleanup()
		{
			try
			{
                if (TestContext.CurrentContext.Result.Outcome != ResultState.Success)
				{
					TestContext.WriteLine(Generic.TestErrorMessage);

					if (LoggingUtility == null || ConfigurationReader == null) return;

					var outputPath = !string.IsNullOrWhiteSpace(ConfigurationReader.GetConfigurationValue(Constants.Configuration.ConfigKeyOutputFileScreenshotsDirectory)) ? $"{ConfigurationReader.GetConfigurationValue(Constants.Configuration.ConfigKeyOutputFileScreenshotsDirectory)}" : $"{TestContext.CurrentContext.TestDirectory}{Constants.Configuration.ScreenshotsDirectory}";

					if (((WebTestManager)TestManager).IsDriverNull) return;

					TestContext.WriteLine($"Attempting to capture screenshot to: {outputPath}");

					var captureScreenShot = Driver.ScreenShotSaveFile(outputPath, TestContext.CurrentContext.Test.Name);

					if (string.IsNullOrWhiteSpace(captureScreenShot)) return;

                    TestContext.AddTestAttachment(captureScreenShot);

					LoggingUtility.Error(captureScreenShot);
				}
				else
				{
					TestContext.WriteLine(Generic.TestSuccessMessage);
				}
			}
			catch (System.Exception ex)
			{
				HandleException(ex);
			}
			finally
			{
				TestManager.OnTestCleanup();
			}
		}

		public IWebDriver Driver => ((WebTestManager)TestManager).Browser;

		public override HttpClient HttpClient => ((WebTestManager)TestManager).HttpClient;
    }
}




