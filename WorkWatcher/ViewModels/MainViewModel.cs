using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            appSettings = DataStorageService.LoadSettings();

            _monitoringSession = new MonitoringSession();
            sessionManagementService = new SessionManagementService(_monitoringSession);
            SessionSwitchButtonCommand = new Command(
                    executeAction: _ => SessionSwitchButtonCommandExecute(),
                    canExecuteFunc: _ => sessionManagementService.isSessionActive
            );
                SettingButtonCmd = new Command(
                        executeAction: _ =>
                        {
                            // 설정 버튼 클릭 시 동작
                            if (settingsWindow != null && settingsWindow.IsVisible)
                            {
                                settingsWindow.Activate(); // 이미 열려있다면 창을 활성화
                            }
                            else
                            {
                                settingsWindow = new SettingsWindow();
                                settingsWindow.DataContext = new SettingsViewModel(appSettings); // 설정 창에 ViewModel 연결
                                settingsWindow.Show(); // 열려있지 않다면 창을 엶
                            }
                        },
                        canExecuteFunc: _ => true
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
                sessionManagementService.StartNewSession();
            }
            else
            {
                sessionManagementService.EndCurrentSession();
            }
        }
    }
}
