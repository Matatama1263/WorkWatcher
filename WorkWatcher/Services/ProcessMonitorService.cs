using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Collections.Generic;
using WorkWatcher.Models;

namespace WorkWatcher.Services
{
    public class ProcessMonitorService
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        private System.Timers.Timer _monitorTimer;
        private string _currentActiveProcess;
        private DateTime _lastCheckTime;
        private List<ProgramInfo> _monitoredPrograms;

        public event EventHandler<ProcessActivityEventArgs> ProcessActivityDetected;

        public ProcessMonitorService()
        {
            _monitorTimer = new System.Timers.Timer(1000); // 1초마다 체크
            _monitorTimer.Elapsed += OnTimerElapsed;
            _monitoredPrograms = new List<ProgramInfo>();
        }

        public void StartMonitoring()
        {
            _lastCheckTime = DateTime.Now;
            _monitorTimer.Start();
        }

        public void StopMonitoring()
        {
            _monitorTimer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var processName = GetActiveProcessName();
            var elapsedTime = DateTime.Now - _lastCheckTime;
            _lastCheckTime = DateTime.Now;

            if (!string.IsNullOrEmpty(processName))
            {
                ProcessActivityDetected?.Invoke(this, new ProcessActivityEventArgs
                {
                    ProcessName = processName,
                    Duration = elapsedTime
                });
            }
        }

        private string GetActiveProcessName()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out int processId);
                Process process = Process.GetProcessById(processId);
                return process.ProcessName;
            }
            catch
            {
                return null;
            }
        }

        public void SetMonitoredPrograms(List<ProgramInfo> programs)
        {
            _monitoredPrograms = programs;
        }
    }

    public class ProcessActivityEventArgs : EventArgs
    {
        public string ProcessName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}