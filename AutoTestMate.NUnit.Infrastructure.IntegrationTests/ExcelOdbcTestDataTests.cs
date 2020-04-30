using AutoTestMate.NUnit.Infrastructure.Attributes;
using AutoTestMate.NUnit.Infrastructure.Core;
using AutoTestMate.NUnit.Infrastructure.Enums;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class ExcelOdbcTestDataTests : TestBase
    {
        //[Test]
        //[ExcelOdbcTestData(TestType.Normal,  @"./Data", "NurseryRhymesBook.xls",  "8", "TableThree", ConnectionStringType =  OdbcConnectionStringType.ExcelXls)]
        //public void EnsureCorrectFieldsAccessed()
        //{
        //    Assert.IsTrue(ConfigurationReader.GetConfigurationValue("RowKey") == "8");
        //    Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldSeven") == "Climbed");
        //    Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldEight") == "Up");
        //    Assert.IsTrue(ConfigurationReader.GetConfigurationValue("FieldNine") == "The");
        //}
    }
}