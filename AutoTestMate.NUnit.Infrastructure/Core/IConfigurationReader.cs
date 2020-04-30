using System.Collections.Generic;
using NUnit.Framework;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
    /// <summary>
    ///     Interface for Framework Configuration Readers
    /// </summary>
    public interface IConfigurationReader
    {
	    void SetTestContext(TestParameters testContext);
        void AddSetting(string key, string value);
        bool UpdateSetting(string key, string value);
        string GetConfigurationValue(string key, bool required = false);
        string LogLevel { get; }
        string LogName { get; }
	    IDictionary<string, string> Settings { get; }
	    IConfiguration AppConfiguration { get; }
        TestParameters TestParameters { get; }
	}
}