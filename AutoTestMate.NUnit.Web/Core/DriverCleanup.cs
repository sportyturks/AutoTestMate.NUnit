using System;
using System.Collections.Generic;
using System.Linq;
using AutoTestMate.NUnit.Infrastructure.Constants;
using AutoTestMate.NUnit.Infrastructure.Core;

namespace AutoTestMate.NUnit.Web.Core
{
	public class DriverCleanup : IDriverCleanup
	{
		public const int MaxKillAttemps = 100;
		private IEnumerable<int> _processIdsBefore;
		private readonly string _browserProcess;
		private readonly string _driverProcess;
		private readonly IConfigurationReader _configurationReader;
		
		public DriverCleanup(string driverProcess, string browserProcess, IProcess process, IConfigurationReader configurationReader)
		{
			_browserProcess = browserProcess;
			_driverProcess = driverProcess;
			_processIdsBefore = new List<int>();
			_configurationReader = configurationReader;
			Process = process;
		}

		public void Dispose()
		{
			CleanBrowserProcesses();
			CleanDriverServerProcesses();
		}

		public void Initialise()
		{
			CleanDriverServerProcesses();
			CleanBrowserProcesses(true);

			_processIdsBefore = Process.GetProcessesByName(_browserProcess);
		}

		public IProcess Process { get; }

		private void CleanBrowserProcesses(bool onInitialise = false)
		{
			try
			{
				var noOfKillAttempts = 0;

				if (string.Equals(Generic.TrueValue.ToLower(), _configurationReader.GetConfigurationValue(Constants.Configuration.ForceKillProcessKey).ToLower()))
				{
					var processIds = Process.GetProcessesByName(_browserProcess.ToLower()).ToList();

					if (processIds.Count <= 0) return;

					//Due to the nature of how processes are killed in windows (parent and children termination of  processes) and how we have already
					//performed a check for processes the do while loop is the most appropriate. We also need to take into account that the account may
					//not be authorised to kill a process that doesnt belong to them
					do
					{
						Process.Kill(processIds.FirstOrDefault());
						processIds = Process.GetProcessesByName(_browserProcess.ToLower()).ToList();
						noOfKillAttempts++;
					} while (processIds.Count > 0 && noOfKillAttempts < MaxKillAttemps);
				}
				else if (!onInitialise)
				{
					var processIdsAfter = Process.GetProcessesByName(_browserProcess);
					var processIds = processIdsAfter.Except(_processIdsBefore).ToList();

					if (processIds.Count <= 0) return;

					//Due to the nature of how processes are killed in windows (parent and children termination of processes) and how we have already
					//performed a check for processes the do while loop is the most appropriate. We also need to take into account that the account may
					//not be authorised to kill a process that doesnt belong to them
					do
					{
						Process.Kill(processIds.FirstOrDefault());
						processIdsAfter = Process.GetProcessesByName(_browserProcess);
						processIds = processIdsAfter.Except(_processIdsBefore).ToList();
						noOfKillAttempts++;
					} while (processIds.Count > 0 && noOfKillAttempts < MaxKillAttemps);
				}
			}
			catch (System.Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void CleanDriverServerProcesses()
		{
			var processes = Process.GetProcessesByName(_driverProcess.ToLower()).ToList();

			if (processes.Count <= 0) return;

			var noOfKillAttempts = 0;
			//Due to the nature of how processes are killed in windows (parent and children termination of processes) and how we have already
			//performed a check for processes the do while loop is the most appropriate. We also need to take into account that the account may
			//not be authorised to kill a process that doesnt belong to them
			do
			{
				Process.Kill(processes.FirstOrDefault());
				processes = Process.GetProcessesByName(_driverProcess.ToLower()).ToList();
				noOfKillAttempts++;
			} while (processes.Count > 0 && noOfKillAttempts < MaxKillAttemps);
		}
	}
}