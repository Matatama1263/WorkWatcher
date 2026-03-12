using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Bases;
using WorkWatcher.Models;
using WorkWatcher.Services;

namespace WorkWatcher.ViewModels
{
    public class StatisticsViewModel : Model
    {
        #region 데이터 바인딩 속성
        public ObservableCollection <MonitoringSession> Sessions { get; set; }
        bool _isAscending = false;

        private MonitoringSession _selectedSession;
        public MonitoringSession SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                OnPropertyChanged(nameof(SelectedSession));
            }
        }

        private Statistics _statistics;
        public Statistics Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                OnPropertyChanged(nameof(Statistics));
            }
        }

        private SeriesCollection _StatisticsTTRCollection;
        public SeriesCollection StatisticsTTRCollection
        {
            get => _StatisticsTTRCollection;
            set
            {
                _StatisticsTTRCollection = value;
                OnPropertyChanged(nameof(StatisticsTTRCollection));
            }
        }

        private SeriesCollection _StatisticsQCRCollection;
        public SeriesCollection StatisticsQCRCollection
        {
            get => _StatisticsQCRCollection;
            set
            {
                _StatisticsQCRCollection = value;
                OnPropertyChanged(nameof(StatisticsQCRCollection));
            }
        }

        private SeriesCollection _selectedSessionTTRCollection;
        public SeriesCollection SelectedSessionTTRCollection
        {
            get => _selectedSessionTTRCollection;
            set
            {
                _selectedSessionTTRCollection = value;
                OnPropertyChanged(nameof(SelectedSessionTTRCollection));
            }
        }

        private SeriesCollection _selectedSessionWPTCollection;
        public SeriesCollection SelectedSessionWPTCollection
        {
            get => _selectedSessionWPTCollection;
            set
            {
                _selectedSessionWPTCollection = value;
                OnPropertyChanged(nameof(SelectedSessionWPTCollection));
            }
        }

        private SeriesCollection _selectedSessionDPTCollection;
        public SeriesCollection SelectedSessionDPTCollection
        {
            get => _selectedSessionDPTCollection;
            set
            {
                _selectedSessionDPTCollection = value;
                OnPropertyChanged(nameof(SelectedSessionDPTCollection));
            }
        }
        #endregion

        public StatisticsViewModel()
        {
            PropertyChanged += UpdateStatisticsCharts;
            PropertyChanged += UpdateSelectedSessionCharts;
            LoadSessions();
            LoadStatistics();
        }

        public void LoadStatistics()
        {
            Statistics = DataStorageService.LoadStatistics();
        }

        public void LoadSessions()
        {
            var sessions = DataStorageService.LoadAllSessions();
            Sessions = new ObservableCollection<MonitoringSession>(sessions);
            SortSessions(false);
        }

        public void AddSession(MonitoringSession session)
        {
            Sessions.Add(session);
            DataStorageService.SaveSession(session);
        }

        public void SortSessions(bool ascending)
        {
            var sorted = ascending 
                ? Sessions.OrderBy(s => s.StartTime).ToList()
                : Sessions.OrderByDescending(s => s.StartTime).ToList();
            
            Sessions.Clear();
            foreach (var session in sorted)
            {
                Sessions.Add(session);
            }
            
            _isAscending = ascending;
            OnPropertyChanged(nameof(Sessions));
        }

        public void SortSessions()
        {
            _isAscending = !_isAscending;

            var sorted = _isAscending
                ? Sessions.OrderBy(s => s.StartTime).ToList()
                : Sessions.OrderByDescending(s => s.StartTime).ToList();

            Sessions.Clear();
            foreach (var session in sorted)
            {
                Sessions.Add(session);
            }
            OnPropertyChanged(nameof(Sessions));
        }

        private void UpdateStatisticsCharts(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Statistics))
                return;

            StatisticsTTRCollection = new SeriesCollection
            {
                new LiveCharts.Wpf.PieSeries
                {
                    Title = "작업 시간",
                    Values = new ChartValues<double> { Statistics.TotalWorkTime.TotalSeconds },
                    DataLabels = true,
                },
                new LiveCharts.Wpf.PieSeries
                {
                    Title = "딴짓 시간",
                    Values = new ChartValues<double> { Statistics.TotalDistractionTime.TotalSeconds },
                    DataLabels = true,
                },
                new LiveCharts.Wpf.PieSeries
                {
                    Title = "기타",
                    Values = new ChartValues<double> { Statistics.TotalComputerTime.TotalSeconds - Statistics.TotalWorkTime.TotalSeconds - Statistics.TotalDistractionTime.TotalSeconds },
                    DataLabels = true,
                }
            };
            double quotaMetPercentage = Statistics.TotalSessionsCount > 0 ? (double)Statistics.QuotaMetCount / Statistics.TotalSessionsCount * 100 : 0;
            StatisticsQCRCollection = new SeriesCollection
            {
                new LiveCharts.Wpf.PieSeries
                {
                    Title = "목표 달성",
                    Values = new ChartValues<double> { quotaMetPercentage },
                    DataLabels = true,
                },
                new LiveCharts.Wpf.PieSeries
                {
                    Title = "목표 미달성",
                    Values = new ChartValues<double> { 100 - quotaMetPercentage },
                    DataLabels = true,
                }
            };
        }

        private void UpdateSelectedSessionCharts(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedSession))
                return;

            SelectedSessionTTRCollection = new SeriesCollection();
            SelectedSessionWPTCollection = new SeriesCollection();
            SelectedSessionDPTCollection = new SeriesCollection();

            if (SelectedSession == null)
            {
                return;
            }

            SelectedSessionTTRCollection .Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "작업 시간",
                Values = new ChartValues<double> { SelectedSession.TotalWorkTime.TotalSeconds },
                DataLabels = true,
            });
            SelectedSessionTTRCollection.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "딴짓 시간",
                Values = new ChartValues<double> { SelectedSession.TotalDistractionTime.TotalSeconds },
                DataLabels = true,
            });
            SelectedSessionTTRCollection.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "기타",
                Values = new ChartValues<double> { SelectedSession.TotalComputerTime.TotalSeconds - SelectedSession.TotalWorkTime.TotalSeconds - SelectedSession.TotalDistractionTime.TotalSeconds },
                DataLabels = true,
            });

            foreach (var program in SelectedSession.WorkPrograms)
            {
                SelectedSessionWPTCollection.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = program.Key,
                    Values = new ChartValues<double> { program.Value.TotalSeconds },
                    DataLabels = true,
                });
            }
            foreach (var program in SelectedSession.DistractionPrograms)
            {
                SelectedSessionDPTCollection.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = program.Key,
                    Values = new ChartValues<double> { program.Value.TotalSeconds },
                    DataLabels = true,
                });
            }
        }
    }
}
