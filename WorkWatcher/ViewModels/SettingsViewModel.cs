using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkWatcher.Bases;
using WorkWatcher.Models;
using WorkWatcher.Services;

namespace WorkWatcher.ViewModels
{
    public class SettingsViewModel : Model
    {
        public AppSettings AppSettings { get; set; }

        // DataGrid에 바인딩할 Observable Collection
        public ObservableCollection<ProgramInfo> MonitoredPrograms { get; set; }
        public ObservableCollection<DistractionProgramInfo> DistractionPrograms { get; set; }

        // 할당량 시간 입력 (시, 분)
        private int _quotaHours;
        public int QuotaHours
        {
            get => _quotaHours;
            set
            {
                _quotaHours = value;
                OnPropertyChanged(nameof(QuotaHours));
            }
        }

        private int _quotaMinutes;
        public int QuotaMinutes
        {
            get => _quotaMinutes;
            set
            {
                _quotaMinutes = value;
                OnPropertyChanged(nameof(QuotaMinutes));
            }
        }

        // 딴짓 허용 시간 입력 (시, 분)
        private int _punishmentHours;
        public int PunishmentHours
        {
            get => _punishmentHours;
            set
            {
                _punishmentHours = value;
                OnPropertyChanged(nameof(PunishmentHours));
            }
        }

        private int _punishmentMinutes;
        public int PunishmentMinutes
        {
            get => _punishmentMinutes;
            set
            {
                _punishmentMinutes = value;
                OnPropertyChanged(nameof(PunishmentMinutes));
            }
        }

        public SettingsViewModel()
        {
            LoadSettings();
        }

        // 설정 로드
        public void LoadSettings()
        {
            AppSettings = DataStorageService.LoadSettings();

            MonitoredPrograms = new ObservableCollection<ProgramInfo>(AppSettings.MonitoredPrograms);
            DistractionPrograms = new ObservableCollection<DistractionProgramInfo>(AppSettings.DistractionPrograms);

            // TimeSpan을 시간과 분으로 분리
            QuotaHours = (int)AppSettings.DailyQuota.QuotaTime.TotalHours;
            QuotaMinutes = AppSettings.DailyQuota.QuotaTime.Minutes;

            PunishmentHours = (int)AppSettings.DailyQuota.PunishmentThreshold.TotalHours;
            PunishmentMinutes = AppSettings.DailyQuota.PunishmentThreshold.Minutes;
        }

        // 설정 저장
        public void SaveSettings()
        {
            // ObservableCollection을 List로 변환
            AppSettings.MonitoredPrograms = MonitoredPrograms.ToList();
            AppSettings.DistractionPrograms = DistractionPrograms.ToList();

            // 시간과 분을 TimeSpan으로 변환
            AppSettings.DailyQuota.QuotaTime = new TimeSpan(QuotaHours, QuotaMinutes, 0);
            AppSettings.DailyQuota.PunishmentThreshold = new TimeSpan(PunishmentHours, PunishmentMinutes, 0);

            DataStorageService.SaveSettings(AppSettings);
        }

        // 작업 프로그램 추가
        public void AddMonitoredProgram(string processName, string pass)
        {
            var program = new ProgramInfo
            {
                ProcessName = processName,
                TotalActiveTime = TimeSpan.Zero
            };

            MonitoredPrograms.Add(program);
        }

        // 작업 프로그램 제거
        public void RemoveMonitoredProgram(ProgramInfo program)
        {
            if (program != null && MonitoredPrograms.Contains(program))
            {
                MonitoredPrograms.Remove(program);
            }
        }

        // 딴짓 프로그램 추가
        public void AddDistractionProgram(string processName, string pass, bool strictMonitoring = false)
        {
            var program = new DistractionProgramInfo
            {
                ProcessName = processName,
                TotalActiveTime = TimeSpan.Zero,
                StrictMonitoring = strictMonitoring
            };

            DistractionPrograms.Add(program);
        }

        // 딴짓 프로그램 제거
        public void RemoveDistractionProgram(DistractionProgramInfo program)
        {
            if (program != null && DistractionPrograms.Contains(program))
            {
                DistractionPrograms.Remove(program);
            }
        }

        // 할당량 유효성 검사
        public bool ValidateQuota()
        {
            return QuotaHours >= 0 && QuotaMinutes >= 0 && QuotaMinutes < 60 &&
                   PunishmentHours >= 0 && PunishmentMinutes >= 0 && PunishmentMinutes < 60;
        }
    }
}
