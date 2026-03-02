using System;
using System.Diagnostics;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

namespace WorkWatcher.Services
{
    public class ProcessBlockerService
    {
        private System.Timers.Timer _blockTimer;
        private HashSet<string> _blockedProcesses;

        public ProcessBlockerService()
        {
            _blockTimer = new System.Timers.Timer(500); // 0.5초마다 체크
            _blockTimer.Elapsed += OnBlockTimerElapsed;
            _blockedProcesses = new HashSet<string>();
        }

        public void StartBlocking(List<string> processNames)
        {
            _blockedProcesses = new HashSet<string>(processNames);
            _blockTimer.Start();
        }

        public void StopBlocking()
        {
            _blockTimer.Stop();
            _blockedProcesses.Clear();
        }

        public void AddBlockedProcess(string processName)
        {
            _blockedProcesses.Add(processName);
        }

        public void RemoveBlockedProcess(string processName)
        {
            _blockedProcesses.Remove(processName);
        }

        private void OnBlockTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                try
                {
                    if (_blockedProcesses.Contains(process.ProcessName))
                    {
                        process.Kill();
                        // 옵션: 사용자에게 알림 표시
                    }
                }
                catch
                {
                    // 프로세스 종료 실패 (권한 부족 등)
                }
            }
        }
    }
}