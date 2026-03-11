using System;
using System.Diagnostics;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace WorkWatcher.Services
{
    public class ProcessBlockerService
    {
        private HashSet<string> _blockedProcesses;
        public bool isActive;
        ManagementEventWatcher watcher;

        public ProcessBlockerService()
        {
            string query = "SELECT * FROM Win32_ProcessStartTrace";
            watcher = new ManagementEventWatcher(query);
            _blockedProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            watcher.EventArrived += (sender, e) =>
            {
                if (!isActive) return;
                string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

                processName = System.IO.Path.GetFileNameWithoutExtension(processName);

                if (_blockedProcesses.Contains(processName))
                    KillProcess(processName);
            };
            watcher.Start();
        }

        ~ProcessBlockerService()
        {
            watcher.Stop();
            watcher.Dispose();
        }

        public void SetBlockedProcesses(List<string> processNames)
        {
            _blockedProcesses = new HashSet<string>(processNames, StringComparer.OrdinalIgnoreCase);
        }

        public void AddBlockedProcess(string processName)
        {
            _blockedProcesses.Add(processName);
        }

        public void RemoveBlockedProcess(string processName)
        {
            _blockedProcesses.Remove(processName);
        }

        public void AddBlockedProcesses(string[] processNames)
        {
            foreach (var name in processNames)
            {
                _blockedProcesses.Add(name);
            }
        }

        public void KillProcess(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    // 예외 처리 (예: 권한 문제)
                    Console.WriteLine($"Failed to kill process {processName}: {ex.Message}");
                }
            }
        }

        public void KillBlockedProcesses(List<string> processNames)
        {
            foreach (var name in processNames)
            {
                var processes = Process.GetProcessesByName(name);
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리 (예: 권한 문제)
                        Console.WriteLine($"Failed to kill process {name}: {ex.Message}");
                    }
                }
            }
        }
    }
}