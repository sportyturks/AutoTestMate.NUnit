using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using AutoTestMate.NUnit.Infrastructure.Core;

namespace AutoTestMate.NUnit.Web.Core
{
	public class WinOsProcess : IProcess
	{
		private readonly ILoggingUtility _loggingUtility;

		public WinOsProcess(ILoggingUtility loggingUtility)
		{
			_loggingUtility = loggingUtility;
		}

		public IEnumerable<int> GetProcessesByName(string name)
		{
			return Process.GetProcessesByName(name).Select(p => p.Id);
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
					KillAllChildProcesses(id);

					process.Kill();
				}
				catch (System.Exception exp)
				{
					var loggingUtility = _loggingUtility;
					loggingUtility.Error("Error while killing process " + exp.Message);
				}

				return;
			}
		}
        private void KillAllChildProcesses(int id)
		{
			foreach (var childProc in GetChildProcesses(id))
			{
				childProc.Kill();
			}
		}

		public List<Process> GetChildProcesses(int parentId)
		{
			var result = new List<Process>();
			var searcher = new ManagementObjectSearcher("Select ProcessId From Win32_Process Where ParentProcessId = " + parentId);
			var processList = searcher.Get();

			foreach (var item in processList)
			{
				result.Add(Process.GetProcessById(Convert.ToInt32(item.GetPropertyValue("ProcessId"))));
			}

			return result;
		}
	}
}