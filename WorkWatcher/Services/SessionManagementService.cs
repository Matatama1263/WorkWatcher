using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Models;

namespace WorkWatcher.Services
{
    public class SessionManagementService
    {
        MonitoringSession currentSession;
        ProcessMonitorService processMonitor;

        public Quota currentQuota;

        // 감시할 프로그램과 방해 프로그램 리스트
        public Dictionary<string, ProgramInfo> _monitoredPrograms;
        public Dictionary<string, DistractionProgramInfo> _distractionPrograms;

        // 프로세스 차단 서비스
        public ProcessBlockerService blocker;

        public bool isSessionActive;

        public SessionManagementService()
        {
            currentSession = new MonitoringSession();
            currentQuota = new Quota();
            blocker = new ProcessBlockerService();

            _monitoredPrograms = new Dictionary<string, ProgramInfo>();
            _distractionPrograms = new Dictionary<string, DistractionProgramInfo>();

            processMonitor = new ProcessMonitorService(this);
            processMonitor.ProcessActivityDetected += OnProcessActivityDetected;
        }

        // 프로그램 리스트 설정 메서드
        public void SetMonitoredPrograms(Dictionary<string, ProgramInfo> programs)
        {
            _monitoredPrograms = programs;
        }

        // 방해 프로그램 리스트 설정 메서드
        public void SetDistractionPrograms(Dictionary<string, DistractionProgramInfo> programs)
        {
            _distractionPrograms = programs;
        }

        public void StartNewSession()
        {
            currentSession.StartTime = DateTime.Now;
            currentSession.EndTime = null;
            currentSession.SessionQuota.QuotaMet = false;
            currentSession.SessionQuota.PunishmentActive = false;
            currentSession.SessionQuota.Punishmented = false;
            currentSession.TotalWorkTime = TimeSpan.Zero;
            currentSession.TotalDistractionTime = TimeSpan.Zero;
            currentSession.TotalComputerTime = TimeSpan.Zero;
            currentSession.ProgramUsage.Clear();

            List<string> distractionProcessNames = new List<string>();
            foreach (var program in _distractionPrograms)
            {
                if (program.Value.StrictMonitoring)
                    distractionProcessNames.Add(program.Key);
            }
            blocker.SetBlockedProcesses(distractionProcessNames);
            blocker.KillBlockedProcesses(distractionProcessNames);
            blocker.isActive = true;

            processMonitor.StartTracking();

            isSessionActive = true;
        }

        public void EndCurrentSession()
        {
            currentSession.EndTime = DateTime.Now;

            blocker.isActive = false;

            processMonitor.StopTracking();

            isSessionActive = false;
        }

        // 프로세스 활동 감지 이벤트 핸들러
        public void OnProcessActivityDetected(object? sender, ProcessActivityEventArgs e)
        {
            // 감지된 프로세스가 방해 프로그램인지 확인
            bool isDistraction = _distractionPrograms.ContainsKey(e.ProcessName);

            // 세션 통계 업데이트

            // 총 컴퓨터 사용 시간 업데이트
            currentSession.TotalComputerTime += e.Duration;

            // 방해 프로그램이면 방해 시간 업데이트, 아니면 작업 시간 업데이트
            if (isDistraction) // 방해 프로그램인 경우
            {
                // 방해 시간 업데이트
                currentSession.TotalDistractionTime += e.Duration;

                // 방해 프로그램의 총 활성 시간 업데이트
                if (_distractionPrograms.ContainsKey(e.ProcessName))
                {
                    _distractionPrograms[e.ProcessName].TotalActiveTime += e.Duration;
                }

                // 처벌시간 이상이면 처벌 활성화
                if ((!currentSession.SessionQuota.QuotaMet) &&
                    (!currentSession.SessionQuota.PunishmentActive) &&
                    currentSession.TotalDistractionTime >= currentQuota.PunishmentThreshold)
                {
                    // 모든 방해 프로그램 차단
                    blocker.KillBlockedProcesses(_distractionPrograms.Keys.ToList());
                    blocker.SetBlockedProcesses(_distractionPrograms.Keys.ToList());

                    currentSession.SessionQuota.PunishmentActive = true;
                    currentSession.SessionQuota.Punishmented = true;
                }
            }
            else // 작업 프로그램인 경우
            {
                // 작업 시간 업데이트
                currentSession.TotalWorkTime += e.Duration;

                // 작업 프로그램의 총 활성 시간 업데이트
                if (_monitoredPrograms.ContainsKey(e.ProcessName))
                {
                    _monitoredPrograms[e.ProcessName].TotalActiveTime += e.Duration;
                }

                // 할당량 충족 여부 업데이트
                if ((!currentSession.SessionQuota.QuotaMet) &&
                    currentSession.TotalWorkTime >= currentQuota.RequiredWorkTime)
                {
                    blocker.isActive = false; // 차단 해제

                    currentSession.SessionQuota.QuotaMet = true;
                    currentSession.SessionQuota.PunishmentActive = false;
                }
            }

            // 프로그램별 사용 시간 업데이트
            if (currentSession.ProgramUsage.ContainsKey(e.ProcessName))
            {
                currentSession.ProgramUsage[e.ProcessName] += e.Duration;
            }
            else
            {
                currentSession.ProgramUsage[e.ProcessName] = e.Duration;
            }
        }
    }
}
