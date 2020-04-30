using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoTestMate.NUnit.Infrastructure.Core;

namespace AutoTestMate.NUnit.Web.Core
{
    public class LinuxOsProcess : IProcess
    {
        public const string UnixPidRegex = @"\w+\s+(\d+).*";

        private readonly ILoggingUtility _loggingUtility;

        public LinuxOsProcess(ILoggingUtility loggingUtility)
        {
            _loggingUtility = loggingUtility;
        }

        public IEnumerable<int> GetProcessesByName(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            var linuxProcessList = FindUnixProcess();
            linuxProcessList = FilterProcessListBy(linuxProcessList, name);
            var pIdList = linuxProcessList.Select(pidString => GetPidFrom(pidString: pidString, pattern: UnixPidRegex)).Where(pid => pid > 0).ToList();
            return pIdList;
        }

        public int GetProcessesById(int id)
        {
            return Process.GetProcessById(id).Id;
        }

        public void Kill(int id)
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (process.Id != id) continue;

                try
                {
                    process.Kill();
                }
                catch (System.Exception exp)
                {
                    _loggingUtility.Error("Error while killing child processes " + exp.Message);
                }

                return;
            }
        }
        private List<string> FindUnixProcess()
        {
            var processStart = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = "-c lsof -i",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process {StartInfo = processStart};
            process.Start();

            var stringValue = process.StandardOutput.ReadToEnd();

            return SplitByLineBreak(stringValue);
        }

        private List<string> SplitByLineBreak(string processLines)
        {
            var processList = new List<string>();

            if (processLines == null) return processList;

            var list = processLines.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            processList.AddRange(collection: list);

            return processList;
        }

        private List<string> FilterProcessListBy(List<string> processList, string filter)
        {
            if (processList == null)
            {
                return new List<string>();
            }

            return filter == null ? processList : processList.FindAll(i => i != null && i.ToLower().Contains(filter.ToLower()));
        }

        public int GetPidFrom(string pidString, string pattern)
        {
            try
            {
                var matches = Regex.Matches(pidString, pattern);
                return matches.Count > 0 ? Convert.ToInt32(matches[0].Groups[1].Value) : 0;
            }
            catch
            {
                _loggingUtility.Warning($"Cannot convert pid {pidString} to integer during driver cleanup process.");
                return 0;
            }
        }
    }
}