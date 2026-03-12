using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection <MonitoringSession> Sessions { get; set; }
        bool _isAscending = true;

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

        public StatisticsViewModel()
        {
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
            SortSessions();
        }

        public void AddSession(MonitoringSession session)
        {
            Sessions.Add(session);
            DataStorageService.SaveSession(session);
        }

        public void SortSessions()
        {
            if (_isAscending)
            {
                var sorted = Sessions.OrderBy(s => s.StartTime).ToList();
                Sessions = new ObservableCollection<MonitoringSession>(sorted);
            }
            else
            {
                var sorted = Sessions.OrderByDescending(s => s.StartTime).ToList();
                Sessions = new ObservableCollection<MonitoringSession>(sorted);
            }
            _isAscending = !_isAscending;
            OnPropertyChanged(nameof(Sessions));
        }
    }
}
