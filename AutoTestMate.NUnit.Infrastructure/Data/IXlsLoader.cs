using System.Collections.Generic;
using System.Data;

namespace AutoTestMate.NUnit.Infrastructure.Data
{
    public interface IXlsLoader
    {
        /// <summary>
        /// Gets os sets the location of the Excel file, e.g. \\data.
        /// </summary>
        /// <remarks>
        /// Local and UNC paths are allowed.
        /// </remarks>
        string XlsFileLocation { get; }

        /// <summary>
        /// Gets or sets the Excel file name, e.g. Test.xsls.
        /// </summary>
        string XlsFileName { get; }

        /// <summary>
        /// Gets or sets the Excel worksheet name.
        /// </summary>
        string SheetName { get; }

        /// <summary>
        /// Gets or sets the row header number.
        /// </summary>
        /// <remarks>
        /// Row number must be between 1 and 1048576.
        /// </remarks>
        int RowHeaderNumber { get; set; }

        DataTable AsDataTable();
        IEnumerable<DataRow> AsDataRows();
        IEnumerable<object[]> AsEnumerable();
        IEnumerable<DataRow[]> AsDataRowArray();
        DataRow GetDataRow(string keyColumnName, object keyValue);
    }
}