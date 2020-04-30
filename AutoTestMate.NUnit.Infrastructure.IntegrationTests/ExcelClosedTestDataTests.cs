using AutoTestMate.NUnit.Infrastructure.Attributes;
using AutoTestMate.NUnit.Infrastructure.Core;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class ExcelClosedTestDataTests : TestBase
    {
        [Test]
        [ExcelClosedTestData( @"./Data", "NurseryRhymesBook.xlsx",  "8", "TableThree")]
        public void EnsureCorrectFieldsAccessed()
        {
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("RowKey") == "8");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldSeven") == "Climbed");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldEight") == "Up");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldNine") == "The");
        }

        [Test]
        [ExcelClosedTestData(@"./Data", "NurseryRhymesBook.xlsx", "8", "TableThree")]
        public void EnsureCorrectFieldsAccessed1()
        {
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("RowKey") == "8");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldSeven") == "Climbed");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldEight") == "Up");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldNine") == "The");
        }
    }
}