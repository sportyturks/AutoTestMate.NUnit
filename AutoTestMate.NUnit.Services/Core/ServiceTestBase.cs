using System;
using System.Net.Http;
using AutoTestMate.NUnit.Infrastructure.Core;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Services.Core
{
    public class ServiceTestBase : TestBase
    {
        #region Initilise/Cleanup

        [SetUp]
        public override void OnTestInitialise()
        {
            try
            {
                TestManager = ServiceTestManager.Instance();
                TestManager.OnTestMethodInitialise(TestParameters);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        
        [TearDown]
        public override void OnTestCleanup()
        {
            base.OnTestCleanup();
        }

        #endregion
        
        public virtual HttpClient HttpClient => ((ServiceTestManager)TestManager).HttpClient;
    }
}