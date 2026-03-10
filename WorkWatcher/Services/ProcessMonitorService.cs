using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        // 활동 감지 타이머
        Timer timer;
        TimeSpan preTickTime;

        // 프로세스 활동 감지 이벤트
        public event EventHandler<ProcessActivityEventArgs> ProcessActivityDetected;

        // 생성자에서 타이머 초기화
        public ProcessMonitorService(SessionManagementService sessionManagement)
        {
            timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        // 타이머 콜백 메서드에서 현재 활성화된 프로세스를 감지하고 이벤트를 발생시킴
        public void TimerCallback(object state)
        {
            var processName = GetActiveProcessName();

            if (!string.IsNullOrEmpty(processName))
            {

                ProcessActivityDetected?.Invoke(this, new ProcessActivityEventArgs
                {
                    ProcessName = processName,
                    // 실제 경과 시간
                    Duration = DateTime.Now.TimeOfDay - preTickTime
                });
            }

            preTickTime = DateTime.Now.TimeOfDay;
        }

        public void StartTracking()
        {
            preTickTime = DateTime.Now.TimeOfDay;
            timer.Change(0, 50);
        }

        public void StopTracking()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        // 현재 활성화된 프로세스의 이름을 가져오는 메서드
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
    }

    public class ProcessActivityEventArgs : EventArgs
    {
        public string ProcessName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}