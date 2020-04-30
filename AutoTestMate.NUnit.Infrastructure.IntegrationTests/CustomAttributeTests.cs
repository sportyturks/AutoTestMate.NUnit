using AutoTestMate.NUnit.Infrastructure.Attributes;
using AutoTestMate.NUnit.Infrastructure.Core;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class CustomAttributeTests : TestBase
    {
        [Test]
        [ConsoleAction(TestType.Normal, "Hello")]
        public void EnsureCorrectFieldsAccessed()
        {
        }
    }
}