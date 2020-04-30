using AutoTestMate.NUnit.Infrastructure.Attributes;
using AutoTestMate.NUnit.Infrastructure.Core;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class ConfigurationReaderTests : TestBase
    {
        [Test]
        [ExcelClosedTestData( @".\Data", "NurseryRhymesBook.xlsx", "8", "TableThree")]
        public void EnsureExcelValuesSet1()
        {
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("RowKey") == "8");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldSeven") == "Climbed");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldEight") == "Up");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldNine") == "The");
        }

        [Test]
        [ExcelClosedTestData(@".\Data", "NurseryRhymesBook.xlsx", "4", "TableTwo")]
        public void EnsureExcelValuesSet2()
        {
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("RowKey") == "4");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldFour") == "Blah");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldFive") == "Blah");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldSix") == "Black");
        }

        [Test]
        [ExcelClosedTestData(@".\Data", "NurseryRhymesBook.xlsx", "3", "TableOne")]
        public void EnsureExcelValuesSet3()
        {
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("RowKey") == "3");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldOne") == "Over");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldTwo") == "The");
            Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldThree") == "Tree");
        }
    }

}