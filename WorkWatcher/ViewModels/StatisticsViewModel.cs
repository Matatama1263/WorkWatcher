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

        public StatisticsViewModel()
        {
            LoadSessions();
        }

        public void LoadSessions()
        {
            var sessions = DataStorageService.LoadAllSessions();
            Sessions = new ObservableCollection<MonitoringSession>(sessions);
            OnPropertyChanged(nameof(Sessions));
        }

        public void AddSession(MonitoringSession session)
        {
            Sessions.Add(session);
            DataStorageService.SaveSession(session);
        }
    }
}
