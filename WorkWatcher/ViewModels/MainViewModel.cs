using LiveCharts;
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


        private SettingsWindow settingsWindow;
        private StatisticsWindow statisticsWindow;

        #region 데이터 바인딩 속성, 커맨드 정의

        public Command SessionSwitchButtonCommand { get; set; }
        public Command SettingButtonCmd { get; set; }
        public Command StatisticsButtonCmd { get; set; }

        private MonitoringSession _monitoringSession;
        public MonitoringSession MonitoringSession
        {
            get => _monitoringSession;
            set
            {
                _monitoringSession = value;
                OnPropertyChanged(nameof(MonitoringSession));
            }
        }

        private SeriesCollection _totalTimeSeriesCollection;
        public SeriesCollection TotalTimeSeriesCollection
        {
            get => _totalTimeSeriesCollection;
            set
            {
                _totalTimeSeriesCollection = value;
                OnPropertyChanged(nameof(TotalTimeSeriesCollection));
            }
        }

        private SeriesCollection _workTimeSeriesCollection;
        public SeriesCollection WorkTimeSeriesCollection
        {
            get => _workTimeSeriesCollection;
            set
            {
                _workTimeSeriesCollection = value;
                OnPropertyChanged(nameof(WorkTimeSeriesCollection));
            }
        }

        private SeriesCollection _distractTimeSeriesCollection;
        public SeriesCollection DistractTimeSeriesCollection
        {
            get => _distractTimeSeriesCollection;
            set
            {
                _distractTimeSeriesCollection = value;
                OnPropertyChanged(nameof(DistractTimeSeriesCollection));
            }
        }

        private TimeSpan _remainQuotaTime;
        public TimeSpan RemainQuotaTime
        {
            get => _remainQuotaTime;
            set
            {
                _remainQuotaTime = value;
                OnPropertyChanged(nameof(RemainQuotaTime));
            }
        }


        private TimeSpan _remainDistractTime;
        public TimeSpan RemainDistractTime
        {
            get => _remainDistractTime;
            set
            {
                _remainDistractTime = value;
                OnPropertyChanged(nameof(RemainDistractTime));
            }
        }

        private string _tWT_Text;
        public string TWT_Text
        {
            get => _tWT_Text;
            set
            {
                _tWT_Text = value;
                OnPropertyChanged(nameof(TWT_Text));
            }
        }

        private string _tDT_Text;
        public string TDT_Text
        {
            get => _tDT_Text;
            set
            {
                _tDT_Text = value;
                OnPropertyChanged(nameof(TDT_Text));
            }
        }

        private string _tCT_Text;
        public string TCT_Text
        {
            get => _tCT_Text;
            set
            {
                _tCT_Text = value;
                OnPropertyChanged(nameof(TCT_Text));
            }
        }

        private string _rQT_Text;
        public string RQT_Text
        {
            get => _rQT_Text;
            set
            {
                _rQT_Text = value;
                OnPropertyChanged(nameof(RQT_Text));
            }
        }

        private string _rDT_Text;
        public string RDT_Text
        {
            get => _rDT_Text;
            set
            {
                _rDT_Text = value;
                OnPropertyChanged(nameof(RDT_Text));
            }
        }
        #endregion


        public MainViewModel()
        {
            _monitoringSession = new MonitoringSession();
            sessionManagementService = new SessionManagementService(_monitoringSession);
            _monitoringSession.PropertyChanged += OnUpdateSessionData;
            settingsWindow = new SettingsWindow(); // 생성자에서 한 번만 생성
            statisticsWindow = new StatisticsWindow();

            SessionSwitchButtonCommand = new Command(
                    executeAction: _ => SessionSwitchButtonCommandExecute(),
                    canExecuteFunc: _ => true
            );
            SettingButtonCmd = new Command(
                executeAction: _ =>
                {
                    if (settingsWindow.IsVisible)
                    {
                        settingsWindow.Activate(); // 이미 열려있다면 창을 활성화
                    }
                    else
                    {
                        settingsWindow.Show(); // 같은 인스턴스 재사용
                    }
                },
                canExecuteFunc: _ => !sessionManagementService.isSessionActive
            );

            StatisticsButtonCmd = new Command(
                executeAction: _ =>
                {
                    // 통계 버튼 클릭 시 동작
                    if (statisticsWindow.IsVisible)
                    {
                        statisticsWindow.Activate(); // 이미 열려있다면 창을 활성화
                    }
                    else
                    {
                        statisticsWindow.Show(); // 열려있지 않다면 창을 엶
                    }
                },
                canExecuteFunc: _ => true
            );

            TotalTimeSeriesCollection = new SeriesCollection();
            TotalTimeSeriesCollection.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "작업 시간",
                Values = new ChartValues<double> { 0 },
                DataLabels = true
            });
            TotalTimeSeriesCollection.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "딴짓 시간",
                Values = new ChartValues<double> { 0 },
                DataLabels = true
            });
            TotalTimeSeriesCollection.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "기타",
                Values = new ChartValues<double> { 0 },
                DataLabels = true
            });
        }

        ~MainViewModel()
        {
            _monitoringSession.PropertyChanged -= OnUpdateSessionData;
            // 뷰모델이 소멸될 때 세션이 활성화되어 있으면 종료
        }

        private void SetupChart()
        {
            // 작업 시간 차트와 딴짓 시간 차트 초기화
            WorkTimeSeriesCollection = new SeriesCollection();
            foreach (var program in appSettings.MonitoredPrograms)
            {
                WorkTimeSeriesCollection.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = program,
                    Values = new ChartValues<double> { 0 },
                    DataLabels = true
                });
            }


            DistractTimeSeriesCollection = new SeriesCollection();
            foreach (var program in appSettings.DistractionPrograms)
            {
                DistractTimeSeriesCollection.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = program.ProcessName,
                    Values = new ChartValues<double> { 0 },
                    DataLabels = true
                });
            }
        }

        private void OnUpdateSessionData(object? sender, PropertyChangedEventArgs e)
        {
            // UI 스레드에서 실행되도록 보장
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MonitoringSession.TotalComputerTime):
                        TCT_Text = $"{MonitoringSession.TotalComputerTime.Hours:D2}:{MonitoringSession.TotalComputerTime.Minutes:D2}:{MonitoringSession.TotalComputerTime.Seconds:D2}";
                        break;
                    case nameof(MonitoringSession.TotalWorkTime):
                        TWT_Text = $"{MonitoringSession.TotalWorkTime.Hours:D2}:{MonitoringSession.TotalWorkTime.Minutes:D2}:{MonitoringSession.TotalWorkTime.Seconds:D2}";
                        RemainQuotaTime = MonitoringSession.SessionQuota.QuotaTime - MonitoringSession.TotalWorkTime;
                        if (RemainQuotaTime < TimeSpan.Zero) RemainQuotaTime = TimeSpan.Zero;
                        RQT_Text = $"{RemainQuotaTime.Hours:D2}:{RemainQuotaTime.Minutes:D2}:{RemainQuotaTime.Seconds:D2}";
                        break;
                    case nameof(MonitoringSession.TotalDistractionTime):
                        TDT_Text = $"{MonitoringSession.TotalDistractionTime.Hours:D2}:{MonitoringSession.TotalDistractionTime.Minutes:D2}:{MonitoringSession.TotalDistractionTime.Seconds:D2}";
                        RemainDistractTime = MonitoringSession.SessionQuota.PunishmentThreshold - MonitoringSession.TotalDistractionTime;
                        if (RemainDistractTime < TimeSpan.Zero) RemainDistractTime = TimeSpan.Zero;
                        RDT_Text = $"{RemainDistractTime.Hours:D2}:{RemainDistractTime.Minutes:D2}:{RemainDistractTime.Seconds:D2}";
                        break;
                }

                UpdateProgramUsageCharts();
                TotalTimeSeriesCollection[0].Values[0] = Math.Round(MonitoringSession.TotalWorkTime.TotalSeconds, 2);
                TotalTimeSeriesCollection[1].Values[0] = Math.Round(MonitoringSession.TotalDistractionTime.TotalSeconds, 2);
                TotalTimeSeriesCollection[2].Values[0] = Math.Round(MonitoringSession.TotalComputerTime.TotalSeconds - MonitoringSession.TotalWorkTime.TotalSeconds - MonitoringSession.TotalDistractionTime.TotalSeconds, 2);
            });
        }

        private void UpdateProgramUsageCharts()
        {
            if (sessionManagementService.currentSession.WorkPrograms == null ||
                sessionManagementService.currentSession.WorkPrograms.Count == 0 ||
                sessionManagementService.currentSession.DistractionPrograms == null ||
                sessionManagementService.currentSession.DistractionPrograms.Count == 0)
                return;

            foreach (var series in WorkTimeSeriesCollection)
            {
                series.Values[0] = Math.Round(sessionManagementService.currentSession.WorkPrograms[series.Title].TotalSeconds, 2);
            }
            foreach (var series in DistractTimeSeriesCollection)
            {
                series.Values[0] = Math.Round(sessionManagementService.currentSession.DistractionPrograms[series.Title].TotalSeconds, 2);
            }
        }

        public void SessionSwitchButtonCommandExecute()
        {
            if (sessionManagementService.isSessionActive)
            {
                bool confirmEnd = MessageBox.Show("현재 세션을 종료하시겠습니까?", "세션 종료 확인", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                if (confirmEnd)
                {
                    sessionManagementService.EndCurrentSession();
                }
            }
            else
            {
                settingsWindow.Hide(); // 세션 시작 시 설정 창 숨김
                bool confirmStart = MessageBox.Show("새 세션을 시작하시겠습니까?\n강한 감시 상태인 프로그램이 모두 종료됩니다.", "세션 시작 확인", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                if (confirmStart)
                {
                    appSettings = DataStorageService.LoadSettings();
                    if (appSettings.MonitoredPrograms.Count == 0)
                    {
                        MessageBox.Show("모니터링할 프로그램이 설정되어 있지 않습니다. 설정에서 프로그램을 추가해주세요.", "설정 필요", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        SetupChart();
                        sessionManagementService.StartNewSession(appSettings);
                    }
                }
            }
        }
    }
}
