using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
    public class TestLogger : ILoggingUtility
    {
        private readonly IConfigurationReader _configurationReader;
	    public const string DefaultFileName = "log.txt";
	    public const string FileOutputDirectory = "OutputFileDirectory";
        public TestLogger(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
            SetupLogger();
        }

        private void SetupLogger()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ConsoleTarget();
            config.AddTarget("Console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("File", fileTarget);

            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

	        var outputDirectory = _configurationReader.GetConfigurationValue(FileOutputDirectory);
	        string outputFile;
			if (!string.IsNullOrWhiteSpace(outputDirectory) && outputDirectory.Contains("/")) //handle relative paths
	        {
		        outputFile = $"{outputDirectory}/{DefaultFileName}";
	        }
			else if (!string.IsNullOrWhiteSpace(outputDirectory) && outputDirectory.Contains(@"\")) //handle absolute paths
	        {
		        outputFile = Path.Combine(Path.GetDirectoryName(outputDirectory),DefaultFileName);
			}
	        else //set default log file
	        {
		        outputFile = "${basedir}/" + DefaultFileName;
	        }

			fileTarget.FileName = outputFile;
			
            fileTarget.Layout = "${message}";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            Logger = LogManager.GetLogger("TestAutomationLogger");
        }

        protected ILogger Logger;

        /// <summary>
        ///     Logs message if log level is Info
        /// </summary>
        public void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        ///     Logs message if log level is Error
        /// </summary>
        public void Error(string message)
        {
            Logger.Error(message);
        }

        /// <summary>
        ///     Logs message if log level is Warning
        /// </summary>
        public void Warning(string message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        ///     Logs message if log level is Debug
        /// </summary>
        public void Debug(string message)
        {
            Logger.Debug(message);
        }
    }
}
