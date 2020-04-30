using System;
using AutoTestMate.NUnit.Infrastructure.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AutoTestMate.NUnit.Infrastructure.Attributes
{
    /// <summary>
    /// [Test][ConsoleAction("Hello")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = true)]
    public class ConsoleActionAttribute : Attribute, ITestAction
    {
        private string _message;
        private ITestManager _testManager;
        private TestParameters _testParameters;

        public ConsoleActionAttribute(TestType testType, string message) { 
            
            _message = message;
            switch (testType)
            {
                case TestType.Normal:
                    _testManager = TestManager.Instance();
                    break;
                case TestType.Service:
                    //_testManager = ServiceTestManager.Instance();
                    break;
                case TestType.Web:
                    //_testManager = WebTestManager.Instance();
                    break;
                default:
                    _testManager = TestManager.Instance();
                    break;
            }

            _testParameters = TestContext.Parameters;
        }

        public void BeforeTest(ITest details)
        {
            WriteToConsole("Before", details);
        }

        public void AfterTest(ITest details)
        {
            WriteToConsole("After", details);
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;

        private void WriteToConsole(string eventMessage, ITest details)
        {
            Console.WriteLine("{0} {1}: {2}, from {3}.{4}.",
                eventMessage,
                details.IsSuite ? "Suite" : "Case",
                _message,
                details.Fixture != null ? details.Fixture.GetType().Name : "{no fixture}",
                details.Method != null ? details.Method.Name : "{no method}");
        }
    }
}