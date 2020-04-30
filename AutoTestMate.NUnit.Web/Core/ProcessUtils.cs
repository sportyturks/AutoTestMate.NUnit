using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AutoTestMate.NUnit.Web.Core
{
    public enum Platform { UNIX, WIN }
    public class ProcessUtils
    {

        public const string UNIX_PID_REGX = @"\w+\s+(\d+).*";
        public const string WIND_PID_REGX = @".*\s+(\d+)";

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (args != null && args.Length == 1)
            {
                findAndKillProcessRuningOn(port: args[0]);
            }
            else
            {
                Console.WriteLine("Illegal port option");
            }
        }

        public static void findAndKillProcessRuningOn(string port)
        {
            List<string> pidList = new List<string>();
            List<string> list = new List<string>();
            switch (getOSName())
            {
                case Platform.UNIX:
                    list = findUnixProcess();
                    list = filterProcessListBy(processList: list, filter: ":" + port);

                    foreach (string pidString in list)
                    {
                        string pid = getPidFrom(pidString: pidString, pattern: UNIX_PID_REGX);

                        if (!String.IsNullOrEmpty(pid))
                        {
                            pidList.Add(pid);
                        }
                    }
                    break;

                case Platform.WIN:
                    list = findWindowsProcess();
                    list = filterProcessListBy(processList: list, filter: ":" + port);

                    foreach (string pidString in list)
                    {
                        string pid = getPidFrom(pidString: pidString, pattern: WIND_PID_REGX);

                        if (!String.IsNullOrEmpty(pid))
                        {
                            pidList.Add(pid);
                        }
                    }
                    break;
                default:
                    Console.WriteLine("No match found");
                    break;
            }

            foreach (string pid in pidList)
            {
                killProcesBy(pidString: pid);
            }
        }

        public static Platform getOSName()
        {
            string os = System.Environment.OSVersion.VersionString;
            Console.WriteLine("OS = {0}", os);

            if (os != null && os.ToLower().Contains("unix"))
            {
                Console.WriteLine("UNXI machine");
                return Platform.UNIX;
            }
            else
            {
                Console.WriteLine("Windows machine");
                return Platform.WIN;
            }
        }

        public static void killProcesBy(string pidString)
        {
            int pid = -1;
            if (pidString != null && int.TryParse(s: pidString, result: out pid))
            {
                Process p = Process.GetProcessById(pid);
                p.Kill();
                Console.WriteLine("Killed pid =" + pidString);
            }
            else
            {
                Console.WriteLine("Process not found for pid =" + pidString);
            }

        }

        public static List<String> findUnixProcess()
        {
            ProcessStartInfo processStart = new ProcessStartInfo();
            processStart.FileName = "bash";
            processStart.Arguments = "-c lsof -i";

            processStart.RedirectStandardOutput = true;
            processStart.UseShellExecute = false;
            processStart.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = processStart;
            process.Start();

            String outstr = process.StandardOutput.ReadToEnd();

            return splitByLineBreak(outstr);
        }

        public static List<String> findWindowsProcess()
        {
            ProcessStartInfo processStart = new ProcessStartInfo();
            processStart.FileName = "netstat.exe";
            processStart.Arguments = "-aon";

            processStart.RedirectStandardOutput = true;
            processStart.UseShellExecute = false;
            processStart.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = processStart;
            process.Start();

            String outstr = process.StandardOutput.ReadToEnd();

            return splitByLineBreak(outstr);
        }

        public static List<string> splitByLineBreak(string processLines)
        {
            List<string> processList = new List<string>();

            if (processLines != null)
            {
                string[] list = processLines.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                processList.AddRange(collection: list);
            }

            return processList;
        }

        public static List<String> filterProcessListBy(List<String> processList,
                                                   String filter)
        {

            if (processList == null)
            {
                return new List<string>();
            }

            if (filter == null)
            {
                return processList;
            }

            return processList.FindAll(i => i != null && i.ToLower().Contains(filter.ToLower()));
        }

        public static String getPidFrom(String pidString, String pattern)
        {
            MatchCollection matches = Regex.Matches(pidString, pattern);

            if (matches != null && matches.Count > 0)
            {
                return matches[0].Groups[1].Value;
            }

            return "";
        }
    }
}
