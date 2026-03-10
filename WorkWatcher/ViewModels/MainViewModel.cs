using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkWatcher.Bases;
using WorkWatcher.Models;
using WorkWatcher.Services;
using WorkWatcher.Views;

namespace WorkWatcher.ViewModels
{
    public class MainViewModel : Model
    {
        private AppSettings appSettings;

        private SessionManagementService sessionManagementService;
        private MonitoringSession _monitoringSession;

        private SettingsWindow settingsWindow;
        private StatisticsWindow statisticsWindow;

        public MainViewModel()
        {
            _monitoringSession = new MonitoringSession();
            sessionManagementService = new SessionManagementService(_monitoringSession);
            SessionSwitchButtonCommand = new Command(
                    executeAction: _ => SessionSwitchButtonCommandExecute(),
                    canExecuteFunc: _ => !sessionManagementService.isSessionActive
            );
            SettingButtonCmd = new Command(
                    executeAction: _ =>
                    {
                        // 다이얼로그 창으로 열기
                        settingsWindow = new SettingsWindow();
                        settingsWindow.Owner = App.Current.MainWindow; // 메인 창을 소유자로 설정
                        settingsWindow.ShowDialog(); // 열려있지 않다면 창을 엶
                    },
                    canExecuteFunc: _ => !sessionManagementService.isSessionActive
            );
    
            StatisticsButtonCmd = new Command(
                    executeAction: _ =>
                    {
                        // 통계 버튼 클릭 시 동작
                        if (statisticsWindow != null && statisticsWindow.IsVisible)
                        {
                            statisticsWindow.Activate(); // 이미 열려있다면 창을 활성화
                        }
                        else
                        {
                            statisticsWindow = new StatisticsWindow();
                            statisticsWindow.Show(); // 열려있지 않다면 창을 엶
                        }
                    },
                    canExecuteFunc: _ => true
            );
        }

        public MonitoringSession MonitoringSession
        {
            get => _monitoringSession;
            set
            {
                _monitoringSession = value;
                OnPropertyChanged(nameof(MonitoringSession));
            }
        }

        public Command SessionSwitchButtonCommand { get; set; }
        public Command SettingButtonCmd { get; set; }
        public Command StatisticsButtonCmd { get; set; }

        public void SessionSwitchButtonCommandExecute()
        {
            if (sessionManagementService.isSessionActive)
            {
                sessionManagementService.EndCurrentSession();
            }
            else
            {
                appSettings = DataStorageService.LoadSettings();
                if (appSettings.MonitoredPrograms.Count == 0)
                {
                    MessageBox.Show("모니터링할 프로그램이 설정되어 있지 않습니다. 설정에서 프로그램을 추가해주세요.", "설정 필요", MessageBoxButton.OK, MessageBoxImage.Warning);
                } else sessionManagementService.StartNewSession(appSettings);
            }
        }
    }
}
