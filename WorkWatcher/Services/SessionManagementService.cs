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
        public MonitoringSession currentSession;
        ProcessMonitorService processMonitor;

        // 감시할 딴짓 프로그램 리스트
        public Dictionary<string, DistractionProgramInfo> _distractionPrograms;

        // 프로세스 차단 서비스
        public ProcessBlockerService blocker;

        public bool isSessionActive;

        public SessionManagementService(MonitoringSession monitoringSession)
        {
            currentSession = monitoringSession;
            blocker = new ProcessBlockerService();

            _distractionPrograms = new Dictionary<string, DistractionProgramInfo>();

            processMonitor = new ProcessMonitorService(this);
            processMonitor.ProcessActivityDetected += OnProcessActivityDetected;
        }

        // 방해 프로그램 리스트 설정 메서드
        public void SetDistractionPrograms(Dictionary<string, DistractionProgramInfo> programs)
        {
            _distractionPrograms = programs;
        }

        public void StartNewSession(AppSettings appSettings)
        {

            currentSession.SessionQuota.QuotaMet = false;
            currentSession.SessionQuota.PunishmentActive = false;
            currentSession.SessionQuota.Punishmented = false;
            currentSession.SessionQuota.QuotaTime = appSettings.DailyQuota.QuotaTime;
            currentSession.SessionQuota.PunishmentThreshold = appSettings.DailyQuota.PunishmentThreshold;
            currentSession.StartTime = DateTime.Now;


            currentSession.WorkPrograms = new Dictionary<string, TimeSpan>();
            foreach (var program in appSettings.MonitoredPrograms)
            {
                currentSession.WorkPrograms.Add(program, TimeSpan.Zero);
            }
            currentSession.DistractionPrograms = new Dictionary<string, TimeSpan>();
            foreach (var program in appSettings.DistractionPrograms)
            {
                currentSession.DistractionPrograms.Add(program.ProcessName, TimeSpan.Zero);
            }
            SetDistractionPrograms(appSettings.DistractionPrograms.ToDictionary(p => p.ProcessName));

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

            DataStorageService.SaveSession(currentSession);

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
                currentSession.DistractionPrograms[e.ProcessName] += e.Duration;
                currentSession.OnPropertyChanged(nameof(currentSession.DistractionPrograms));

                // 처벌시간 이상이면 처벌 활성화
                if ((!currentSession.SessionQuota.QuotaMet) &&
                    (!currentSession.SessionQuota.PunishmentActive) &&
                    currentSession.TotalDistractionTime >= currentSession.SessionQuota.PunishmentThreshold)
                {
                    // 모든 방해 프로그램 차단
                    blocker.KillBlockedProcesses(_distractionPrograms.Keys.ToList());
                    blocker.SetBlockedProcesses(_distractionPrograms.Keys.ToList());

                    currentSession.SessionQuota.PunishmentActive = true;
                    currentSession.SessionQuota.Punishmented = true;
                }
            }
            else if (currentSession.WorkPrograms.ContainsKey(e.ProcessName)) // 작업 프로그램인 경우
            {
                // 작업 시간 업데이트
                currentSession.TotalWorkTime += e.Duration;
                currentSession.WorkPrograms[e.ProcessName] += e.Duration;
                currentSession.OnPropertyChanged(nameof(currentSession.WorkPrograms));


                // 할당량 충족 여부 업데이트
                if ((!currentSession.SessionQuota.QuotaMet) &&
                    currentSession.TotalWorkTime >= currentSession.SessionQuota.QuotaTime)
                {
                    blocker.isActive = false; // 차단 해제

                    currentSession.SessionQuota.QuotaMet = true;
                    currentSession.SessionQuota.PunishmentActive = false;
                }
            }

            // ProgramUsage가 변경되었음을 알림
        }
    }
}
