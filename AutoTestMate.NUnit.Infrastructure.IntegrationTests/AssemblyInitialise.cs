using AutoTestMate.NUnit.Infrastructure.Core;
using AutoTestMate.NUnit.Web.Core;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.IntegrationTests
{
    [SetUpFixture]
    public class AssemblyInitialise
    {
        [OneTimeSetUp]
        public void Init()
        {
            TestManager.Instance().OnInitialiseAssemblyDependencies(TestContext.Parameters);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TestManager.Instance().OnDisposeAssemblyDependencies();
        }
    }
}