using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AutoTestMate.NUnit.Infrastructure.Helpers;
using ClosedXML.Excel;

namespace AutoTestMate.NUnit.Infrastructure.Data
{
    public class XlsLoader : IXlsLoader
    {
        private DataTable _table;

        public XlsLoader(string xlsFileLocation, string xlsFileName, string sheetName, int rowHeaderNumber = -1)
        {
            XlsFileLocation = xlsFileLocation;
            XlsFileName = xlsFileName;
            SheetName = sheetName;
            RowHeaderNumber = rowHeaderNumber;
        }

        /// <summary>
        /// Gets os sets the location of the Excel file, e.g. \\data.
        /// </summary>
        /// <remarks>
        /// Local and UNC paths are allowed.
        /// </remarks>
        public string XlsFileLocation { get; private set; }

        /// <summary>
        /// Gets or sets the Excel file name, e.g. Test.xsls.
        /// </summary>
        public string XlsFileName { get; private set; }

        /// <summary>
        /// Gets or sets the Excel worksheet name.
        /// </summary>
        public string SheetName { get; private set; }

        /// <summary>
        /// Gets or sets the row header number.
        /// </summary>
        /// <remarks>
        /// Row number must be between 1 and 1048576.
        /// </remarks>
        public int RowHeaderNumber { get; set; }

        public DataTable AsDataTable()
        {
            var dt = new DataTable();
            var fileLocation = XlsFileLocation;

            if (fileLocation.StartsWith(".\\"))
            {
                fileLocation = fileLocation.Replace(".\\", FileHelper.GetCurrentExecutingDirectory() + "\\");
            }

            var fileName = Path.Combine(fileLocation, XlsFileName);
            using (var workbook = new XLWorkbook(fileName))
            {
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == SheetName);
                if (worksheet != null)
                {
                    if (RowHeaderNumber <= 0)
                    {
                        PopulateRowsWithoutHeader(dt, worksheet);
                    }
                    else
                    {
                        PopulateRowsWithHeader(dt, worksheet);
                    }
                }
            }
            _table = dt;
            return dt;
        }

        public IEnumerable<DataRow> AsDataRows()
        {
            if (_table == null)
            {
                _table = AsDataTable();
            }

            return new List<DataRow>();
        }

        public IEnumerable<object[]> AsEnumerable()
        {
            var rows = AsDataRows();
            return rows.Select(row => row.ItemArray);
        }

        public IEnumerable<DataRow[]> AsDataRowArray()
        {

            var rows = AsDataRows();
            return rows.Select(row => new[] { row });
        }

        public DataRow GetDataRow(string keyColumnName, object keyValue)
        { 
            if (_table == null)
            {
                _table = AsDataTable();
            }

            if (_table.Columns.Contains(keyColumnName))
            {
                var row = _table.Select($"{keyColumnName} = '{keyValue}'").FirstOrDefault();
                return row;
            }

            return null;
        }

        private void PopulateRowsWithoutHeader(DataTable dt, IXLWorksheet worksheet)
        {
            // Use column letters to create data table columns
            foreach (var col in worksheet.Columns())
            {
                var dtCol = new DataColumn(col.ColumnLetter(), typeof(string));
                dt.Columns.Add(dtCol);
            }

            foreach (var row in worksheet.Rows())
            {
                var dtRow = dt.NewRow();
                foreach (var col in worksheet.Columns())
                {
                    dtRow[col.ColumnLetter()] = row.Cell(col.ColumnLetter()).Value?.ToString();
                }

                dt.Rows.Add(dtRow);
            }
        }

        private void PopulateRowsWithHeader(DataTable dt, IXLWorksheet worksheet)
        {
            // Use the first worksheet row data to create the data table columns
            var headerRow = worksheet.Row(RowHeaderNumber);

            foreach (var cell in headerRow.Cells())
            {
                if (cell.Value != null)
                {
                    var dtCol = new DataColumn(cell.Value.ToString(), typeof(string));
                    dt.Columns.Add(dtCol);
                }
            }

            foreach (var row in worksheet.Rows())
            {
                if (row.RowNumber() != RowHeaderNumber) // exclude the header
                {
                    var dtRow = dt.NewRow();
                    foreach (var cell in headerRow.Cells())
                    {
                        if (cell.Value != null)
                        {
                            var colName = cell.Value.ToString();
                            var colLetter = cell.WorksheetColumn().ColumnLetter();
                            dtRow[colName] = row.Cell(colLetter).Value?.ToString();
                        }
                    }
                    dt.Rows.Add(dtRow);
                }
            }
        }
    }
}