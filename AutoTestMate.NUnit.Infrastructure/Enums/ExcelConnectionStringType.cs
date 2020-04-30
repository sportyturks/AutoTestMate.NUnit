using System.ComponentModel;

namespace AutoTestMate.NUnit.Infrastructure.Enums
{
    public enum ExcelConnectionStringType
    {
        [Description("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=1\";")]
        Excel2007 = 1,
        [Description("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\";")]
        Excel2003 = 2
    }

    /// <summary>
    /// Driver={Microsoft Excel Driver (*.xls)};DBQ=c:\bin\book1.xls
    /// </summary>
    public enum OdbcConnectionStringType
    {
        [Description("Driver={{{0} (*.xlsx)}};DBQ={1};")]
        ExcelXlsx = 1,
        [Description("Driver={{{0} (*.xls)}};DBQ={1};")]
        ExcelXls = 2,
        [Description("Driver={{{0} (*.xls)}};DBQ={1};")]
        Excel = 3,

    }
}