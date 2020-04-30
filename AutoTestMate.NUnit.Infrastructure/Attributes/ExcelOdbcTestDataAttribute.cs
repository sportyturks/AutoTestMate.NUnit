using System;
using System.Data;
using System.Data.Odbc;
using AutoTestMate.NUnit.Infrastructure.Constants;
using AutoTestMate.NUnit.Infrastructure.Core;
using AutoTestMate.NUnit.Infrastructure.Enums;
using AutoTestMate.NUnit.Infrastructure.Extensions;
using AutoTestMate.NUnit.Infrastructure.Helpers;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AutoTestMate.NUnit.Infrastructure.Attributes
{
	/// <summary>
	/// Used to manage test data for a specific test method and rowkey that does not perform a data driven test in a row sequence manner. 
	/// Data is stored via excel spreadsheets. A RowKey is provided to match a particular test 
	/// 
	/// Sample usage: 
	/// 
	/// Attribute on class:
	/// [ExcelTestData(ConnectionStringType = ExcelConnectionStringType.Excel2007,FileLocation = @".\data", FileName = "orgsite2.xlsx", RowKey = "TC88888", SheetName = "Sheet2$")]
	/// 
	/// Retrieve data:
	/// Driver.Instance().TestDataReader.GetConfigurationValue("Org", false)
	/// </summary>
	[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ExcelOdbcTestDataAttribute : Attribute, ITestAction
	{
		#region Private Variables

		private string _dataSourceSettingName; //i.e.System.Data.Odbc
		private string _sheetName; //i.e. Sheet1$
		private string _rowKey; //i.e. TC99999, used to get a single row of data
		private string _fileLocation; //i.e. TC99999, used to get a single row of data
		private string _fileName; //i.e. TC99999, used to get a single row of data
		private string _driverName = "Microsoft Excel Driver";
        private ITestManager _testManager;

		#endregion

        public ExcelOdbcTestDataAttribute(string fileLocation, string fileName, string rowKey, string sheetName, string driverName = "Microsoft Excel Driver", OdbcConnectionStringType connectionStringType = OdbcConnectionStringType.Excel)
        {
            FileLocation = fileLocation;
            FileName = fileName;
            RowKey = rowKey;
            SheetName = sheetName;
            DriverName = driverName;
            ConnectionStringType = connectionStringType;
        }

		#region Properties

		//	/// <summary>
		//	/// The excel data Source setting name, i.e.System.Data.Odbc 
		//	/// </summary>
		public string DataSourceSettingName
		{
			get => !string.IsNullOrWhiteSpace(_dataSourceSettingName) ? _dataSourceSettingName : string.Empty;
			set => _dataSourceSettingName = value;
		}

		/// <summary>
		/// The excel data Source connectionstring, for more info see https://www.connectionstrings.com/excel/, 
		/// </summary>
		public OdbcConnectionStringType ConnectionStringType { get; set; }

		/// <summary>
		/// The excel sheet name, i.e. Sheet1$
		/// </summary>
		public string SheetName
		{
			get => !string.IsNullOrWhiteSpace(_sheetName) ? _sheetName : string.Empty;
			set => _sheetName = value;
		}

		/// <summary>
		/// Get the excel row with the key, i.e. TC99999, used to get a single row of data with the name TC99999
		/// </summary>
		public string RowKey
		{
			get => !string.IsNullOrWhiteSpace(_rowKey) ? _rowKey : string.Empty;
			set => _rowKey = value;
		}

		/// <summary>
		/// Get the excel row with the key, i.e. TC99999, used to get a single row of data with the name TC99999
		/// </summary>
		public string FileLocation
		{
			get => !string.IsNullOrWhiteSpace(_fileLocation) ? _fileLocation : FileHelper.GetCurrentExecutingDirectory();
			set => _fileLocation = value;
		}

		/// <summary>
		/// Get the excel row with the key, i.e. TC99999, used to get a single row of data with the name TC99999
		/// </summary>
		public string FileName
		{
			get => !string.IsNullOrWhiteSpace(_fileName) ? _fileName : string.Empty;
			set => _fileName = value;
		}
		
		/// <summary>
		/// Get the excel row with the key, i.e. TC99999, used to get a single row of data with the name TC99999
		/// </summary>
		public string DriverName
		{
			get => !string.IsNullOrWhiteSpace(_driverName) ? _driverName : "Microsoft Excel Driver";
			set => _driverName = value;
		}

		#endregion

		#region Methods

		private DataTable ReadFile()
		{
			var dt = new DataTable();
			var fileLocation = FileLocation;

			if (fileLocation.StartsWith(".\\") || fileLocation.StartsWith("./"))
			{
				fileLocation = fileLocation.Replace(".\\", FileHelper.GetCurrentExecutingDirectory() + "\\").Replace("./", FileHelper.GetCurrentExecutingDirectory() + "\\");
			}

			string connectionString;
			switch (ConnectionStringType)
			{
				case OdbcConnectionStringType.ExcelXls:
					connectionString = string.Format(OdbcConnectionStringType.ExcelXls.GetDescription(), $"{_driverName}", $"{fileLocation}\\{FileName}");
					break;
				case OdbcConnectionStringType.ExcelXlsx:
					connectionString = string.Format(OdbcConnectionStringType.ExcelXlsx.GetDescription(), $"{_driverName}", $"{fileLocation}\\{FileName}");
					break;
				default:
					connectionString = string.Format(OdbcConnectionStringType.Excel.GetDescription(), $"{_driverName}", $"{fileLocation}\\{FileName}");
					break;
			}

			using var connection = new OdbcConnection(connectionString);
			{
				var query = $"SELECT * FROM [{SheetName}$]";
				using var command = new OdbcCommand(query);
				{
					command.Connection = connection;
					connection.Open();
					var olDataReader = command.ExecuteReader(CommandBehavior.Default);
					dt.Load(olDataReader);
				}
			}

			return dt;
		}

        private void UpdateConfigurationReader(DataTable dt, IConfigurationReader configurationReader)
        {
            var columnKeyIndex = ExcelTestData.ExcelMissingKeyIndex;

            for (var k = 0; k < dt.Columns.Count; k++)
            {
                if (string.Equals(dt.Columns[k].ColumnName.Trim().ToLower(),
                    ExcelTestData.RowKey.Trim().ToLower()))
                {
                    columnKeyIndex = k;
                }
            }

            if (columnKeyIndex == ExcelTestData.ExcelMissingKeyIndex) return;

            for (var r = 0; r < dt.Rows.Count; r++)
            {
                if (!string.Equals(dt.Rows[r][columnKeyIndex].ToString().ToLower(), RowKey.ToLower())) continue;

                for (var c = 0; c < dt.Columns.Count; c++)
                {
                    configurationReader.AddSetting(dt.Columns[c].ColumnName, dt.Rows[r][c].ToString());
                }

                return;
            }
        }

		#endregion

		public void BeforeTest(ITest test)
        {
            Exception exp = null;
            try
            {
                var testBase = (TestBase)test.Fixture;
                _testManager = testBase.TestManager;

				_testManager.SetTestParameters(TestContext.Parameters);
                var configurationReader = _testManager.ConfigurationReader;
                var dt = ReadFile();
                UpdateConfigurationReader(dt, configurationReader);
                _testManager.UpdateConfigurationReader(configurationReader);
            }
            catch (Exception ex)
            {
                exp = ex;
            }

            if (exp != null)
            {
                throw exp;
            }
		}
        public void AfterTest(ITest test)
        {
            var testBase = (TestBase)test.Fixture;
            _testManager = testBase.TestManager;
			_testManager.UpdateConfigurationReader(null);
		}
        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
	}
}
