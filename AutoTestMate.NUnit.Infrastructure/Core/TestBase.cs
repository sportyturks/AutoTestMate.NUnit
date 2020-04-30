using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
    public abstract class TestBase
	{
		[SetUp]
		public virtual void OnTestInitialise()
		{
			try
			{
				TestManager = Core.TestManager.Instance();
				TestManager.OnTestMethodInitialise(TestParameters);
            }
			catch (Exception ex)
			{
				if (LoggingUtility == null || ConfigurationReader == null) throw;

				LoggingUtility.Error(Exceptions.Exception.ExceptionMsgSetupError + ex.Message);

				throw;
			}
		}

        [TearDown]
        public virtual void OnTestCleanup()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome != ResultState.Success)
                {
                    try
                    {
                        TestContext.WriteLine(Constants.Generic.TestErrorMessage);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }
                else
                {
                    TestContext.WriteLine(Constants.Generic.TestSuccessMessage);
                }
            }
            catch (Exception exp)
            {
                HandleException(exp);
            }
            finally
            {
                TestManager.OnTestCleanup();
            }
        }

        public virtual void HandleException(Exception exp)
		{
			if (TestManager.LoggingUtility == null || TestManager.ConfigurationReader == null) throw exp;

			LoggingUtility.Error(Exceptions.Exception.ExceptionMsgSetupError + exp.Message);
			TestContext.WriteLine(Exceptions.Exception.ExceptionMsgSetupError + exp.Message);

            throw exp;
		}

		public virtual IConfigurationReader ConfigurationReader => TestManager.ConfigurationReader;

		public virtual ILoggingUtility LoggingUtility => TestManager.LoggingUtility;

        public virtual ITestManager TestManager { get; set; }

        public virtual TestParameters TestParameters => TestManager.TestParameters;
    }
}