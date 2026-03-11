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
        DateTime lastTickTime;
        private readonly object lockObject = new object();

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
            DateTime currentTime;
            DateTime previousTime;
            string processName;

            // 스레드 안전하게 시간 정보 가져오기
            lock (lockObject)
            {
                currentTime = DateTime.Now;
                previousTime = lastTickTime;
                lastTickTime = currentTime;
            }

            // 프로세스 이름 가져오기 (lock 외부에서 실행)
            processName = GetActiveProcessName();

            if (!string.IsNullOrEmpty(processName))
            {
                TimeSpan duration = currentTime - previousTime;

                // 비정상적인 시간 차이 필터링 (예: 시스템 절전 모드 복귀)
                if (duration.TotalSeconds > 0 && duration.TotalSeconds < 10)
                {
                    ProcessActivityDetected?.Invoke(this, new ProcessActivityEventArgs
                    {
                        ProcessName = processName,
                        Duration = duration
                    });
                }
            }
        }

        public void StartTracking()
        {
            lock (lockObject)
            {
                lastTickTime = DateTime.Now;
            }
            timer.Change(0, 100);
        }

        public void StopTracking()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        // 현재 활성화된 프로세스의 이름을 가져오는 메서드
        private string GetActiveProcessName()
        {
            Process process = null;
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero)
                    return null;

                GetWindowThreadProcessId(hwnd, out int processId);
                process = Process.GetProcessById(processId);
                return process.ProcessName;
            }
            catch
            {
                return null;
            }
            finally
            {
                // 프로세스 객체 해제
                process?.Dispose();
            }
        }

        // IDisposable 패턴 구현
        public void Dispose()
        {
            timer?.Dispose();
        }
    }

    public class ProcessActivityEventArgs : EventArgs
    {
        public string ProcessName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}