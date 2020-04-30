using System.Diagnostics.CodeAnalysis;

namespace AutoTestMate.NUnit.Infrastructure.Constants
{
    [ExcludeFromCodeCoverage]
    public class ExcelTestData
    {
        public const string SheetNameKey = "SheetName";
        public const string DataSourceSettingKey = "DataSourceSetting";
        public const string RowKey = "RowKey";
        public const string ConnectionStringKey = "ConnectionString";
        public const string ExcelFileLocationKey = "FileLocation";
        public const string ExcelFileNameKey = "FileName";
        public const int ExcelHeadingRow = 0;
        public const int ExcelMissingKeyIndex = -1;
    }
}
